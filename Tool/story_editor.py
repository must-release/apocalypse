import tkinter as tk
from tkinter import ttk, filedialog, messagebox
import xml.etree.ElementTree as ET
import os

# ---------------------------
# 내부 데이터 구조 정의
# 각 노드는 dict 형태로 저장하며, 반드시 "type" 키로 타입을 구분함.
# Dialogue: {"type": "Dialogue", "character": str, "text": str}
# Effect:   {"type": "Effect", "action": str, "duration": int}
# Choice:   {"type": "Choice", "prevDialogue": {"character": str, "text": str}, "options": [ {"branchId": str, "text": str}, ... ]}
# ---------------------------

class StoryEditorApp(tk.Frame):
    def __init__(self, master):
        super().__init__(master)
        self.master.title("스토리 스크립트 에디터")
        self.pack(fill=tk.BOTH, expand=True)
        
        # 최소 창 크기 설정
        self.master.minsize(1100, 600)
        
        self.block_drag_start_index = None  # Initialize drag operation attribute

        # 내부 데이터
        self.blocks = []  # 각 요소는 {"branchId": str, "entries": list} 형식
        self.selected_block_index = None  # 현재 선택된 블록 인덱스
        self.selected_entry_index = None  # 현재 선택된 엔트리 인덱스

        self.create_menu()
        self.create_widgets()

    def create_menu(self):
        menubar = tk.Menu(self.master)
        filemenu = tk.Menu(menubar, tearoff=0)
        filemenu.add_command(label="새로 만들기", command=self.new_file)
        filemenu.add_command(label="불러오기", command=self.load_xml)
        filemenu.add_command(label="저장하기", command=self.save_xml)
        menubar.add_cascade(label="파일", menu=filemenu)
        self.master.config(menu=menubar)

    def create_widgets(self):
        # 좌측: 블록 리스트 패널
        self.left_frame = tk.Frame(self, bd=2, relief=tk.SUNKEN)
        self.left_frame.place(relx=0, rely=0, relwidth=0.2, relheight=1)
        
        self.block_tree = ttk.Treeview(self.left_frame, columns=("Name", "Delete"), show="headings", selectmode="browse")
        self.block_tree.heading("Name", text="스토리 블록")
        self.block_tree.heading("Delete", text="")
        self.block_tree.column("Name", anchor="w", stretch=False)
        self.block_tree.column("Delete", width=30, minwidth=30, anchor="e", stretch=False)
        self.block_tree.pack(fill=tk.BOTH, expand=True)
        
        # 이 두 이벤트 바인딩을 추가하여 열 크기를 사용자가 변경하는 것을 막습니다.
        self.block_tree.bind("<ButtonPress-1>", self.disable_tree_resize, add="+")
        self.block_tree.bind("<B1-Motion>", self.disable_tree_resize, add="+")
        
        self.block_tree.bind("<Configure>", self.on_block_tree_configure)
        self.block_tree.bind("<<TreeviewSelect>>", self.on_block_select)
        self.block_tree.bind("<ButtonRelease-1>", self.on_block_delete)
        self.block_tree.bind("<Double-Button-1>", self.edit_block_name)
        self.block_tree.bind("<ButtonPress-1>", self.on_block_button_press, add="+")
        self.block_tree.bind("<B1-Motion>", self.on_block_button_drag, add="+")
        self.block_tree.bind("<ButtonRelease-1>", self.on_block_button_release, add="+")

        tk.Button(self.left_frame, text="블록 추가", command=self.add_block).pack(fill=tk.X, pady=5)

        # --- 중앙: 엔트리 패널 부분 ---
        self.center_frame = tk.Frame(self, bd=2, relief=tk.SUNKEN)
        self.center_frame.place(relx=0.2, rely=0, relwidth=0.4, relheight=1)

        self.entry_tree = ttk.Treeview(self.center_frame, columns=("Name", "Delete"), show="headings", selectmode="browse")
        self.entry_tree.heading("Name", text="스토리 엔트리")
        self.entry_tree.heading("Delete", text="") 
        self.entry_tree.column("Name", anchor="w", stretch=False)
        self.entry_tree.column("Delete", width=30, minwidth=30, anchor="e", stretch=False)
        self.entry_tree.pack(fill=tk.BOTH, expand=True)

        self.entry_tree.bind("<ButtonPress-1>", self.disable_tree_resize, add="+")
        self.entry_tree.bind("<B1-Motion>", self.disable_tree_resize, add="+")

        self.entry_tree.bind("<Configure>", self.on_entry_tree_configure)        
        self.entry_tree.bind("<<TreeviewSelect>>", self.on_entry_select)
        self.entry_tree.bind("<ButtonRelease-1>", self.on_entry_delete)
        self.entry_tree.bind("<ButtonPress-1>", self.on_entry_button_press, add="+")
        self.entry_tree.bind("<B1-Motion>", self.on_entry_button_drag, add="+")
        self.entry_tree.bind("<ButtonRelease-1>", self.on_entry_button_release, add="+")

        entry_button_frame = tk.Frame(self.center_frame)
        entry_button_frame.pack(fill=tk.X, pady=5)
        self.entry_type_var = tk.StringVar(value="Dialogue")
        entry_type_dropdown = ttk.Combobox(
            entry_button_frame, textvariable=self.entry_type_var, state="readonly",
            values=["Dialogue", "Effect", "Choice"], width=10
        )
        entry_type_dropdown.pack(side=tk.LEFT, padx=2)
        tk.Button(entry_button_frame, text="엔트리 추가", command=self.add_entry_to_block).pack(side=tk.LEFT, padx=2)

        # --- 우측: 편집 패널 ---
        self.right_frame = tk.Frame(self, bd=2, relief=tk.SUNKEN)
        self.right_frame.place(relx=0.6, rely=0, relwidth=0.4, relheight=1)
        
        self.editor_frame = tk.Frame(self.right_frame)
        self.editor_frame.pack(fill=tk.BOTH, expand=True)
        self.editor_frame.grid_columnconfigure(0, weight=1)
        self.editor_frame.grid_columnconfigure(1, weight=1)
        
        
    def disable_tree_resize(self, event):
        # event.widget가 클릭된 Treeview, event.x, event.y를 사용하여 영역 판단
        region = event.widget.identify_region(event.x, event.y)
        if region == "separator":
            # separator 영역에서의 클릭 및 드래그 이벤트를 차단해서 사용자가 열 너비를 변경하지 못하도록 함
            return "break"

    # ===== 블록 관련 =====
    def add_block(self):
        branch_id = f"Block{len(self.blocks) + 1}"
        self.blocks.append({"branchId": branch_id, "entries": []})
        self.refresh_block_list()
        new_index = len(self.blocks) - 1
        self.selected_block_index = new_index
        self.block_tree.selection_set(str(new_index))
        self.block_tree.focus(str(new_index))
        self.refresh_entry_list()

    def refresh_block_list(self):
        for i in self.block_tree.get_children():
            self.block_tree.delete(i)
        for idx, block in enumerate(self.blocks):
            self.block_tree.insert("", "end", iid=str(idx), values=(f"{idx + 1}. {block['branchId']}", "✖️"))

    def on_block_select(self, event):
        selected = self.block_tree.selection()
        if selected:
            self.selected_block_index = int(selected[0])
            self.refresh_entry_list()
        else:
            self.selected_block_index = None
            for i in self.entry_tree.get_children():
                self.entry_tree.delete(i)

    def edit_block_name(self, event):
        # 더블 클릭한 영역이 "cell" 영역인지 확인
        region = self.block_tree.identify("region", event.x, event.y)
        if region != "cell":
            return
        # 편집은 "Name" 열(#1)에서만 작동
        col = self.block_tree.identify_column(event.x)
        if col != "#1":
            return
        item = self.block_tree.identify_row(event.y)
        if not item:
            return
        # "Name" 열의 셀 bbox를 얻습니다.
        bbox = self.block_tree.bbox(item, column="Name")
        if not bbox:
            return
        x, y, width, height = bbox
        # 현재 아이템의 "Name" 값 추출 (예: "1. BlockName" -> BlockName 부분)
        current_text = self.block_tree.item(item, "values")[0]
        parts = current_text.split(". ", 1)
        if len(parts) == 2:
            block_name = parts[1]
        else:
            block_name = current_text
        
        # Treeview 위에 Entry 위젯을 오버레이합니다.
        self.edit_entry = tk.Entry(self.block_tree)
        self.edit_entry.insert(0, block_name)
        self.edit_entry.place(x=x, y=y, width=width, height=height)
        self.edit_entry.focus_set()
        # 엔터키 또는 포커스 아웃 시 편집 완료 처리
        self.edit_entry.bind("<Return>", lambda e, item=item: self.finish_edit_block_name(item))
        self.edit_entry.bind("<FocusOut>", lambda e, item=item: self.finish_edit_block_name(item))

    def finish_edit_block_name(self, item):
        if not hasattr(self, "edit_entry") or self.edit_entry is None:
            return
        new_text = self.edit_entry.get().strip()
        if new_text:
            idx = int(item)
            self.blocks[idx]["branchId"] = new_text
        self.edit_entry.destroy()
        self.edit_entry = None
        self.refresh_block_list()
        
    def on_block_delete(self, event):
        region = self.block_tree.identify("region", event.x, event.y)
        if region == "cell":
            col = self.block_tree.identify_column(event.x)  # 예: '#2'가 두 번째 열
            if col == "#2":
                # 해당 셀의 아이템(인덱스)
                item = self.block_tree.identify_row(event.y)
                if item and messagebox.askyesno("삭제 확인", "블록을 삭제하겠습니까?"):
                    idx = int(item)
                    del self.blocks[idx]
                    if self.selected_block_index == idx:
                        self.selected_block_index = None
                        # 엔트리 및 편집 패널 초기화
                        for i in self.entry_tree.get_children():
                            self.entry_tree.delete(i)
                        self.clear_editor()
                    self.refresh_block_list()
                    self.refresh_entry_list()
                    
    def on_block_tree_configure(self, event):
        total_width = self.block_tree.winfo_width()
        delete_width = int(self.block_tree.column("Delete", "width"))
        new_width = max(total_width - delete_width, 0)
        self.block_tree.column("Name", width=new_width)
        
    def on_block_button_press(self, event):
        self.block_drag_start_item = self.block_tree.identify_row(event.y)

    def on_block_button_drag(self, event):
        current_item = self.block_tree.identify_row(event.y)
        if current_item and self.block_drag_start_item and current_item != self.block_drag_start_item:
            try:
                start_index = int(self.block_drag_start_item)
                current_index = int(current_item)
            except ValueError:
                return
            
            self.blocks[start_index], self.blocks[current_index] = self.blocks[current_index], self.blocks[start_index]
            self.block_drag_start_item = current_item
            self.refresh_block_list()
            self.block_tree.selection_set(current_item)
            self.block_tree.focus(current_item)

    def on_block_button_release(self, event):
        self.block_drag_start_item = None

    # ===== 엔트리 관련 =====
    def refresh_entry_list(self):
        for i in self.entry_tree.get_children():
            self.entry_tree.delete(i)
        if self.selected_block_index is not None:
            block = self.blocks[self.selected_block_index]
            for idx, entry in enumerate(block["entries"]):
                display_text = self.get_display_text(entry)
                self.entry_tree.insert("", "end", iid=str(idx), values=(f"{idx + 1}. {display_text}", "✖️"))

    def add_entry_to_block(self):
        if self.selected_block_index is None:
            messagebox.showwarning("경고", "블록을 먼저 선택하세요.")
            return

        entry_type = self.entry_type_var.get()
        if entry_type == "Dialogue":
            # 현재 선택된 블록의 엔트리들 중에서, 마지막으로 생성된 Dialogue 엔트리의 character 값을 찾음
            current_entries = self.blocks[self.selected_block_index]["entries"]
            default_character = "독백"
            # 뒤에서부터 순회하며 Dialogue 타입이고, character 값이 비어있지 않으면 사용
            for e in reversed(current_entries):
                if e["type"] == "Dialogue" and e.get("character", ""):
                    default_character = e["character"]
                    break
            new_entry = {"type": "Dialogue", "character": default_character, "text": ""}
        elif entry_type == "Effect":
            new_entry = {"type": "Effect", "action": "", "duration": 0}
        elif entry_type == "Choice":
            new_entry = {"type": "Choice", "prevDialogue": {"character": "", "text": ""}, "options": []}
        else:
            return

        # 새 엔트리를 추가
        self.blocks[self.selected_block_index]["entries"].append(new_entry)
        # 리스트 박스를 갱신
        self.refresh_entry_list()
        self.selected_entry_index = len(self.blocks[self.selected_block_index]["entries"]) - 1
        self.entry_tree.selection_set(str(self.selected_entry_index))
        self.entry_tree.focus(str(self.selected_entry_index))
        self.show_editor_for_index(self.selected_entry_index)

    def on_entry_select(self, event):
        selected = self.entry_tree.selection()
        if selected:
            self.selected_entry_index = int(selected[0])
            self.show_editor_for_index(self.selected_entry_index)
        else:
            self.selected_entry_index = None
            self.clear_editor()
            
    def on_entry_delete(self, event):
        region = self.entry_tree.identify("region", event.x, event.y)
        if region == "cell":
            col = self.entry_tree.identify_column(event.x)
            if col == "#2":
                item = self.entry_tree.identify_row(event.y)
                if item and messagebox.askyesno("삭제 확인", "엔트리를 삭제하겠습니까?"):
                    idx = int(item)
                    if self.selected_block_index is not None:
                        del self.blocks[self.selected_block_index]["entries"][idx]
                        self.refresh_entry_list()
                        self.clear_editor()

    def get_display_text(self, entry):
        """
        엔트리 데이터를 화면에 표시할 텍스트로 변환합니다.
        """
        if entry["type"] == "Dialogue":
            return f"Dialogue: {entry.get('character', '')} - {entry.get('text', '')}"
        elif entry["type"] == "Effect":
            return f"Effect: {entry.get('action', '')} (Duration: {entry.get('duration', 0)})"
        elif entry["type"] == "Choice":
            branch_ids = [opt.get("branchId", "") for opt in entry.get("options", [])]
            return f"Choice: [{', '.join(branch_ids)}]"
        else:
            return "Unknown Entry Type"
        
    def on_entry_tree_configure(self, event):
        total_width = self.entry_tree.winfo_width()
        delete_width = int(self.entry_tree.column("Delete", "width"))
        new_width = max(total_width - delete_width, 0)
        self.entry_tree.column("Name", width=new_width)
        
    def on_entry_button_press(self, event):
        self.entry_drag_start_item = self.entry_tree.identify_row(event.y)

    def on_entry_button_drag(self, event):
        current_item = self.entry_tree.identify_row(event.y)
        if current_item and self.entry_drag_start_item and current_item != self.entry_drag_start_item:
            try:
                start_index = int(self.entry_drag_start_item)
                current_index = int(current_item)
            except ValueError:
                return
            # Swap entries in internal data list for the currently selected block
            if self.selected_block_index is not None:
                entries = self.blocks[self.selected_block_index]["entries"]
                entries[start_index], entries[current_index] = entries[current_index], entries[start_index]
                self.entry_drag_start_item = current_item
                self.refresh_entry_list()
                self.entry_tree.selection_set(current_item)
                self.entry_tree.focus(current_item)

    def on_entry_button_release(self, event):
        self.entry_drag_start_item = None

    # ===== 편집 패널 =====
    def clear_editor(self):
        for widget in self.editor_frame.winfo_children():
            widget.destroy()

    def show_editor_for_index(self, index):
        self.clear_editor()
        if self.selected_block_index is None or index is None:
            return

        block = self.blocks[self.selected_block_index]
        entry = block["entries"][index]
        self.editor_vars = {}

        # row 0: 엔트리 타입 표시
        row = 0
        tk.Label(self.editor_frame, text=entry["type"]).grid(row=row, column=0, columnspan=2, sticky="w")
        row += 1

        t = entry["type"]
        if t == "Dialogue":
            row = self.show_editor_dialogue(entry, row)
        elif t == "Effect":
            row = self.show_editor_effect(entry, row)
        elif t == "Choice":
            row = self.show_editor_choice(entry, row)

        tk.Button(self.editor_frame, text="변경 저장", command=self.save_editor_changes).grid(row=row, column=0, columnspan=2, pady=5)

    def show_editor_dialogue(self, entry, row):
        tk.Label(self.editor_frame, text="캐릭터:").grid(row=row, column=0, sticky="e")
        char_var = tk.StringVar(value=entry.get("character", ""))
        char_combo = ttk.Combobox(self.editor_frame, textvariable=char_var, values=["독백", "나", "소녀", "중개상"], state="readonly", width=5)
        char_combo.grid(row=row, column=1, sticky="w")
        self.editor_vars["character"] = char_var
        row += 1

        tk.Label(self.editor_frame, text="대사:").grid(row=row, column=0, sticky="ne")
        text_widget = tk.Text(self.editor_frame, height=5, wrap=tk.WORD, width=40, font=("TKfixedFont", 10))
        text_widget.grid(row=row, column=1, sticky="w")
        text_widget.insert("1.0", entry.get("text", ""))
        self.editor_vars["text"] = text_widget
        row += 1
        return row

    def show_editor_effect(self, entry, row):
        tk.Label(self.editor_frame, text="액션:").grid(row=row, column=0, sticky="e")
        action_var = tk.StringVar(value=entry.get("action", ""))
        action_combo = ttk.Combobox(self.editor_frame, textvariable=action_var, width=5,
                                    values=["FadeIn", "FadeOut", "Shake"], state="readonly")
        action_combo.grid(row=row, column=1, sticky="w")
        self.editor_vars["action"] = action_var
        row += 1

        tk.Label(self.editor_frame, text="지속 시간:").grid(row=row, column=0, sticky="e")
        duration_var = tk.StringVar(value=str(entry.get("duration", 0.0)))
        tk.Entry(self.editor_frame, textvariable=duration_var, width=5).grid(row=row, column=1, sticky="w")
        self.editor_vars["duration"] = duration_var
        row += 1
        return row

    def show_editor_choice(self, entry, row):
        # 저장용: 현재 편집 중인 Choice 엔트리 저장
        self.editor_vars["choice_entry"] = entry

        # 옵션들을 보여줄 Treeview 생성 (세 개의 열: BranchId, Option, Delete)
        columns = ("BranchId", "Option", "Delete")
        self.choice_option_tree = ttk.Treeview(self.editor_frame, columns=columns, show="headings", selectmode="browse", height=5)
        self.choice_option_tree.heading("BranchId", text="BranchId")
        self.choice_option_tree.heading("Option", text="Option")
        self.choice_option_tree.heading("Delete", text="")
        self.choice_option_tree.column("BranchId", width=60, minwidth=60, anchor="w", stretch=False)
        self.choice_option_tree.column("Option", anchor="w", stretch=False)
        self.choice_option_tree.column("Delete", width=30, minwidth=30, anchor="center", stretch=False)
        self.choice_option_tree.grid(row=row, column=0, columnspan=2, sticky="nsew")

        # Populate Treeview with current options
        self.refresh_choice_option(entry)

        self.choice_option_tree.bind("<Configure>", self.on_choice_option_tree_configure)
        self.choice_option_tree.bind("<ButtonRelease-1>", self.on_choice_option_click)
        self.choice_option_tree.bind("<ButtonPress-1>", self.disable_tree_resize, add="+")
        self.choice_option_tree.bind("<B1-Motion>", self.disable_tree_resize, add="+")
        self.choice_option_tree.bind("<Double-Button-1>", self.edit_choice_option, add="+")
        
        row += 1
        tk.Button(self.editor_frame, text="옵션 추가", command=self.add_choice_option).grid(row=row, column=0, columnspan=2, pady=5)
        row += 1
        return row

    def refresh_choice_option(self, choice_entry):
        # 먼저 기존 항목 삭제
        for item in self.choice_option_tree.get_children():
            self.choice_option_tree.delete(item)
        options = choice_entry.get("options", [])
        for idx, opt in enumerate(options):
            self.choice_option_tree.insert("", "end", iid=str(idx),
                                            values=(opt.get("branchId", ""), opt.get("text", ""), "✖️"))
    
    def on_choice_option_click(self, event):
        region = self.choice_option_tree.identify("region", event.x, event.y)
        if region != "cell":
            return
        col = self.choice_option_tree.identify_column(event.x)  # 예: "#3"가 세 번째 열
        if col != "#3":
            return
        item = self.choice_option_tree.identify_row(event.y)
        if not item:
            return
        if messagebox.askyesno("삭제 확인", "이 옵션을 삭제하시겠습니까?"):
            idx = int(item)
            choice_entry = self.editor_vars.get("choice_entry")
            if choice_entry is not None:
                try:
                    choice_entry["options"].pop(idx)
                except Exception:
                    pass
                self.refresh_choice_option(choice_entry)
                
    def on_choice_option_tree_configure(self, event):
        # 전체 tree의 현재 너비 구하기
        total_width = self.choice_option_tree.winfo_width()
        # BranchId와 Delete 열의 고정 너비를 동적으로 가져옴
        branch_width = int(self.choice_option_tree.column("BranchId", "width"))
        delete_width = int(self.choice_option_tree.column("Delete", "width"))
        # Option 열은 전체 너비에서 나머지 부분 할당 (음수가 되지 않도록 처리)
        option_width = max(total_width - branch_width - delete_width, 0)
        self.choice_option_tree.column("Option", width=option_width)
        
    def edit_choice_option(self, event):
        # Check that the event occurred in a cell
        region = self.choice_option_tree.identify("region", event.x, event.y)
        if region != "cell":
            return
        # Identify the column; allow editing only for BranchId (#1) or Option (#2)
        col = self.choice_option_tree.identify_column(event.x)
        if col not in ("#1", "#2"):
            return
        # Identify the row (item id)
        item = self.choice_option_tree.identify_row(event.y)
        if not item:
            return
        # Get the bounding box for the cell in the given column
        bbox = self.choice_option_tree.bbox(item, column=col)
        if not bbox:
            return
        x, y, width, height = bbox
        # Get the current cell text; "values" returns tuple: (BranchId, Option, Delete)
        values = self.choice_option_tree.item(item, "values")
        current_text = ""
        if col == "#1":
            current_text = values[0]
        elif col == "#2":
            current_text = values[1]
        # Create an Entry widget over the cell
        self.edit_entry_choice = tk.Entry(self.choice_option_tree)
        self.edit_entry_choice.insert(0, current_text)
        self.edit_entry_choice.place(x=x, y=y, width=width, height=height)
        self.edit_entry_choice.focus_set()
        # Bind Return and FocusOut events to finish editing
        self.edit_entry_choice.bind("<Return>", lambda e, item=item, col=col: self.finish_edit_choice_option(item, col))
        self.edit_entry_choice.bind("<FocusOut>", lambda e, item=item, col=col: self.finish_edit_choice_option(item, col))

    def finish_edit_choice_option(self, item, col):
        if not hasattr(self, "edit_entry_choice") or self.edit_entry_choice is None:
            return
        new_text = self.edit_entry_choice.get().strip()
        try:
            idx = int(item)
        except ValueError:
            self.edit_entry_choice.destroy()
            self.edit_entry_choice = None
            return
        # Get the currently editing Choice entry from editor_vars
        choice_entry = self.editor_vars.get("choice_entry")
        if choice_entry is not None:
            options = choice_entry.get("options", [])
            if idx < len(options):
                if col == "#1":  # Update BranchId
                    options[idx]["branchId"] = new_text
                elif col == "#2":  # Update Option text
                    options[idx]["text"] = new_text
                choice_entry["options"] = options
        self.edit_entry_choice.destroy()
        self.edit_entry_choice = None
        # Refresh the choice option Treeview to reflect changes
        self.refresh_choice_option(self.editor_vars.get("choice_entry"))
    
    def add_choice_option(self):
        choice_entry = self.editor_vars.get("choice_entry")
        if choice_entry is not None:
            choice_entry.setdefault("options", []).append({"branchId": "common", "text": ""})
            self.refresh_choice_option(choice_entry)

    def save_editor_changes(self):
        if self.selected_block_index is None or self.selected_entry_index is None:
            return

        block = self.blocks[self.selected_block_index]
        entry = block["entries"][self.selected_entry_index]
        t = entry["type"]

        if t == "Dialogue":
            entry["character"] = self.editor_vars["character"].get()
            entry["text"] = self.editor_vars["text"].get("1.0", tk.END).strip()
        elif t == "Effect":
            entry["action"] = self.editor_vars["action"].get()
            try:
                entry["duration"] = float(self.editor_vars["duration"].get())
            except ValueError:
                entry["duration"] = 0.0
        elif t == "Choice":
            # Choice 엔트리의 옵션은 Treeview 편집 이벤트 핸들러에서 이미 업데이트되고 있으므로,
            # 별도의 Text 입력 위젯으로부터 읽어오지 않고 아무 작업도 하지 않습니다.
            pass

        self.refresh_entry_list()

    # ===== XML 저장 및 불러오기 =====
    def save_xml(self):
        if not self.blocks:
            messagebox.showwarning("저장", "저장할 내용이 없습니다.")
            return

        file_path = filedialog.asksaveasfilename(defaultextension=".xml",
                                                filetypes=[("XML 파일", "*.xml")],
                                                title="XML 파일 저장")
        if not file_path:
            return

        # 검증 단계: 각 블록에 대해 Choice 엔트리의 옵션 검증 실시
        for block_idx, block in enumerate(self.blocks):
            # 현재 블록의 branchId
            current_branch_id = block["branchId"]
            # 현재 블록 이후에 존재하는 블록들의 branchId 리스트
            later_branch_ids = [b["branchId"] for b in self.blocks[block_idx+1:]]
            # 엔트리 번호 (1부터 시작)
            for block_idx, block in enumerate(self.blocks):
                current_branch_id = block["branchId"]
                later_branch_ids = [b["branchId"] for b in self.blocks[block_idx+1:]]
                for i, entry in enumerate(block["entries"]):
                    if entry["type"] == "Choice":
                        for opt in entry.get("options", []):
                            opt_branch = opt.get("branchId", "")
                            if opt_branch.lower() == "common":
                                continue
                            if opt_branch not in later_branch_ids:
                                error_msg = f"[{block_idx+1}. {current_branch_id} : {i+1}. Choice] branchId '{opt_branch}'가 다음 스토리 블록에 존재하지 않습니다."
                                messagebox.showerror("저장 오류", error_msg)
                                return
                        last_dialogue = None
                        for j in range(i):
                            if block["entries"][j]["type"] == "Dialogue":
                                last_dialogue = block["entries"][j]
                        if last_dialogue is None:
                            error_msg = f"[{block_idx+1}. {current_branch_id} : {i+1}. Choice] 앞에 Dialogue가 존재하지 않습니다."
                            messagebox.showerror("저장 오류", error_msg)
                            return
                        else:
                            entry["prevDialogue"] = {
                                "character": last_dialogue.get("character", ""),
                                "text": last_dialogue.get("text", "")
                            }

        # XML 생성 코드 (기존과 동일)
        root_elem = ET.Element("StoryBlocks")
        for block in self.blocks:
            block_elem = ET.SubElement(root_elem, "Block", {"BranchId": block["branchId"]})
            for entry in block["entries"]:
                if entry["type"] == "Dialogue":
                    dialogue_elem = ET.SubElement(block_elem, "Dialogue")
                    char_elem = ET.SubElement(dialogue_elem, "Character")
                    char_elem.text = entry.get("character", "")
                    text_elem = ET.SubElement(dialogue_elem, "Text")
                    text_elem.text = entry.get("text", "")
                elif entry["type"] == "Effect":
                    effect_elem = ET.SubElement(block_elem, "Effect", {
                        "Action": entry.get("action", ""),
                        "Duration": str(entry.get("duration", 0))
                    })
                elif entry["type"] == "Choice":
                    choice_elem = ET.SubElement(block_elem, "Choice")
                    prev = entry.get("prevDialogue", {})
                    prev_elem = ET.SubElement(choice_elem, "PrevDialogue")
                    pd_char = ET.SubElement(prev_elem, "Character")
                    pd_char.text = prev.get("character", "")
                    pd_text = ET.SubElement(prev_elem, "Text")
                    pd_text.text = prev.get("text", "")
                    opts_elem = ET.SubElement(choice_elem, "Options")
                    for opt in entry.get("options", []):
                        opt_elem = ET.SubElement(opts_elem, "Option", {"BranchId": opt.get("branchId", "")})
                        opt_elem.text = opt.get("text", "")
        xml_string = ET.tostring(root_elem, encoding="unicode")
        from xml.dom.minidom import parseString
        pretty_xml = parseString(xml_string).toprettyxml(indent="  ")

        with open(file_path, "w", encoding="utf-8") as file:
            file.write(pretty_xml)

        messagebox.showinfo("저장", f"XML 파일이 저장되었습니다:\n{file_path}")

    def load_xml(self):
        file_path = filedialog.askopenfilename(filetypes=[("XML 파일", "*.xml")], title="XML 파일 불러오기")
        if not file_path:
            return

        try:
            tree = ET.parse(file_path)
            root = tree.getroot()

            self.blocks = []
            for block_elem in root.findall("Block"):
                block = {"branchId": block_elem.get("BranchId", ""), "entries": []}
                for entry_elem in block_elem:
                    if entry_elem.tag == "Dialogue":
                        entry = {
                            "type": "Dialogue",
                            "character": entry_elem.find("Character").text or "",
                            "text": entry_elem.find("Text").text or ""
                        }
                    elif entry_elem.tag == "Effect":
                        entry = {
                            "type": "Effect",
                            "action": entry_elem.get("Action", ""),
                            "duration": float(entry_elem.get("Duration", 0.0))
                        }
                    elif entry_elem.tag == "Choice":
                        prev_elem = entry_elem.find("PrevDialogue")
                        prev_dialogue = {
                            "character": prev_elem.find("Character").text or "",
                            "text": prev_elem.find("Text").text or ""
                        }
                        options = []
                        for opt_elem in entry_elem.find("Options").findall("Option"):
                            options.append({
                                "branchId": opt_elem.get("BranchId", ""),
                                "text": opt_elem.text or ""
                            })
                        entry = {
                            "type": "Choice",
                            "prevDialogue": prev_dialogue,
                            "options": options
                        }
                    else:
                        continue
                    block["entries"].append(entry)
                self.blocks.append(block)

            self.refresh_block_list()
            self.refresh_entry_list()
            self.selected_block_index = None
            self.selected_entry_index = None
            self.clear_editor()

            messagebox.showinfo("불러오기", f"XML 파일이 성공적으로 불러와졌습니다:\n{file_path}")
        except Exception as e:
            messagebox.showerror("오류", f"XML 파일을 불러오는 중 오류가 발생했습니다:\n{e}")

    def new_file(self):
        if messagebox.askyesno("새로 만들기", "현재 작업 내용을 저장하지 않고 새 파일을 만드시겠습니까?"):
            self.blocks = []
            self.selected_block_index = None
            self.selected_entry_index = None
            self.refresh_block_list()
            self.refresh_entry_list()
            self.clear_editor()
            messagebox.showinfo("새로 만들기", "새 파일이 생성되었습니다.")

# ===== 메인 함수 =====
def main():
    root = tk.Tk()
    app = StoryEditorApp(root)
    root.geometry("1000x600")
    root.mainloop()

if __name__ == "__main__":
    main()