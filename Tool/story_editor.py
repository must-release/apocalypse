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
        self.master.title("스토리 스크립트 XML 에디터")
        self.pack(fill=tk.BOTH, expand=True)
        
        # 최소 창 크기 설정
        self.master.minsize(800, 600)
        
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
        
        tk.Label(self.left_frame, text="스토리 블록").pack()
        self.block_listbox = tk.Listbox(self.left_frame, height=20, exportselection=False)
        self.block_listbox.pack(fill=tk.BOTH, expand=True)
        self.block_listbox.bind("<<ListboxSelect>>", self.on_block_select)
        self.block_listbox.bind("<ButtonPress-1>", self.on_block_button_press)
        self.block_listbox.bind("<B1-Motion>", self.on_block_button_motion)
        self.block_listbox.bind("<ButtonRelease-1>", self.on_block_button_release)
        self.block_listbox.bind("<Double-Button-1>", self.edit_block_name)
        
        # 블록 추가 버튼
        tk.Button(self.left_frame, text="블록 추가", command=self.add_block).pack(fill=tk.X, pady=5)
        
        # 중앙: 엔트리 리스트 패널
        self.center_frame = tk.Frame(self, bd=2, relief=tk.SUNKEN)
        self.center_frame.place(relx=0.2, rely=0, relwidth=0.4, relheight=1)
        
        tk.Label(self.center_frame, text="스토리 엔트리").pack()
        self.entry_listbox = tk.Listbox(self.center_frame, height=20, exportselection=False)
        self.entry_listbox.pack(fill=tk.BOTH, expand=True)
        self.entry_listbox.bind("<<ListboxSelect>>", self.on_entry_select)
        
        entry_button_frame = tk.Frame(self.center_frame)
        entry_button_frame.pack(fill=tk.X, pady=5)
        self.entry_type_var = tk.StringVar(value="Dialogue")
        entry_type_dropdown = ttk.Combobox(
            entry_button_frame, textvariable=self.entry_type_var, state="readonly",
            values=["Dialogue", "Effect", "Choice"], width=10
        )
        entry_type_dropdown.pack(side=tk.LEFT, padx=2)
        tk.Button(entry_button_frame, text="엔트리 추가", command=self.add_entry_to_block).pack(side=tk.LEFT, padx=2)
        
        # 우측: 편집 패널
        self.right_frame = tk.Frame(self, bd=2, relief=tk.SUNKEN)
        self.right_frame.place(relx=0.6, rely=0, relwidth=0.4, relheight=1)
        
        tk.Label(self.right_frame, text="편집").pack(anchor="w")
        self.editor_frame = tk.Frame(self.right_frame)
        self.editor_frame.pack(fill=tk.BOTH, expand=True)

    # ===== 블록 관련 =====
    def add_block(self):
        branch_id = f"Block{len(self.blocks) + 1}"
        self.blocks.append({"branchId": branch_id, "entries": []})
        self.refresh_block_list()

    def refresh_block_list(self):
        self.block_listbox.delete(0, tk.END)
        for idx, block in enumerate(self.blocks):
            self.block_listbox.insert(tk.END, f"{idx + 1}. {block['branchId']}")

    def on_block_select(self, event):
        selection = self.block_listbox.curselection()
        if selection:
            self.selected_block_index = int(selection[0])
            self.refresh_entry_list()
        else:
            self.selected_block_index = None
            self.entry_listbox.delete(0, tk.END)

    def edit_block_name(self, event):
        # 더블클릭한 항목의 인덱스를 구합니다.
        index = self.block_listbox.nearest(event.y)
        bbox = self.block_listbox.bbox(index)
        if not bbox:
            return
        # 기존 bbox에서 x, y, w, h를 얻지만, Entry의 width는 전체 Listbox 너비로 설정합니다.
        x, y, w, h = bbox
        listbox_width = self.block_listbox.winfo_width()
        # 약간의 여백을 위해 4픽셀 정도를 빼줍니다.
        entry_width = listbox_width - 4

        # 현재 항목의 텍스트를 가져옵니다.
        current_text = self.block_listbox.get(index)
        parts = current_text.split(". ", 1)
        if len(parts) == 2:
            block_name = parts[1]
        else:
            block_name = current_text

        # Listbox 위에 Entry 위젯을 오버레이 합니다.
        self.edit_entry = tk.Entry(self.block_listbox)
        self.edit_entry.insert(0, block_name)
        # x를 0으로 설정하여 Listbox 전체 너비에 맞게 배치
        self.edit_entry.place(x=0, y=y, width=entry_width, height=h)
        self.edit_entry.focus_set()
        # 엔터키나 포커스 아웃 시 편집 완료 처리
        self.edit_entry.bind("<Return>", lambda e, idx=index: self.finish_edit_block_name(idx))
        self.edit_entry.bind("<FocusOut>", lambda e, idx=index: self.finish_edit_block_name(idx))

    def finish_edit_block_name(self, index):
        new_text = self.edit_entry.get().strip()
        if new_text:
            self.blocks[index]["branchId"] = new_text
        self.edit_entry.destroy()
        self.edit_entry = None
        self.refresh_block_list()

    # ===== 엔트리 관련 =====
    def refresh_entry_list(self):
        self.entry_listbox.delete(0, tk.END)
        if self.selected_block_index is not None:
            block = self.blocks[self.selected_block_index]
            for idx, entry in enumerate(block["entries"]):
                display_text = self.get_display_text(entry)
                self.entry_listbox.insert(tk.END, f"{idx + 1}. {display_text}")

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
        # 새 엔트리의 인덱스를 계산 (리스트의 마지막 항목)
        new_index = len(self.blocks[self.selected_block_index]["entries"]) - 1
        # 선택 상태를 새 엔트리로 설정
        self.selected_entry_index = new_index
        self.entry_listbox.select_clear(0, tk.END)
        self.entry_listbox.select_set(new_index)
        self.entry_listbox.activate(new_index)
        # 편집 패널에 새 엔트리의 편집 화면 표시
        self.show_editor_for_index(new_index)

    def on_entry_select(self, event):
        selection = self.entry_listbox.curselection()
        if selection:
            self.selected_entry_index = int(selection[0])
            self.show_editor_for_index(self.selected_entry_index)
        else:
            self.selected_entry_index = None
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
            options_count = len(entry.get("options", []))
            return f"Choice: {entry.get('prevDialogue', {}).get('text', '')} ({options_count} options)"
        else:
            return "Unknown Entry Type"

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
        text_widget = tk.Text(self.editor_frame, height=5, wrap=tk.WORD, width=50)
        text_widget.grid(row=row, column=1, sticky="w")
        text_widget.insert("1.0", entry.get("text", ""))
        self.editor_vars["text"] = text_widget
        row += 1
        return row

    def show_editor_effect(self, entry, row):
        tk.Label(self.editor_frame, text="액션:").grid(row=row, column=0, sticky="e")
        action_var = tk.StringVar(value=entry.get("action", ""))
        tk.Entry(self.editor_frame, textvariable=action_var).grid(row=row, column=1, sticky="w")
        self.editor_vars["action"] = action_var
        row += 1

        tk.Label(self.editor_frame, text="지속 시간:").grid(row=row, column=0, sticky="e")
        duration_var = tk.StringVar(value=str(entry.get("duration", 0)))
        tk.Entry(self.editor_frame, textvariable=duration_var, width=10).grid(row=row, column=1, sticky="w")
        self.editor_vars["duration"] = duration_var
        row += 1
        return row

    def show_editor_choice(self, entry, row):
        # 이전 대화 편집
        tk.Label(self.editor_frame, text="이전 대화").grid(row=row, column=0, columnspan=2, sticky="w")
        row += 1
        prev = entry.get("prevDialogue", {})
        tk.Label(self.editor_frame, text="캐릭터:").grid(row=row, column=0, sticky="e")
        prev_char_var = tk.StringVar(value=prev.get("character", ""))
        tk.Entry(self.editor_frame, textvariable=prev_char_var).grid(row=row, column=1, sticky="w")
        self.editor_vars["prev_character"] = prev_char_var
        row += 1

        tk.Label(self.editor_frame, text="대사:").grid(row=row, column=0, sticky="e")
        prev_text_var = tk.StringVar(value=prev.get("text", ""))
        tk.Entry(self.editor_frame, textvariable=prev_text_var, width=40).grid(row=row, column=1, sticky="w")
        self.editor_vars["prev_text"] = prev_text_var
        row += 1

        # 옵션 편집 – 각 줄 "BranchId: 옵션 내용" 형태
        tk.Label(self.editor_frame, text="옵션 (각 줄: BranchId: 옵션 내용)").grid(row=row, column=0, columnspan=2, sticky="w")
        row += 1
        options_text = ""
        for option in entry.get("options", []):
            options_text += f"{option.get('branchId', '')}: {option.get('text', '')}\n"
        options_widget = tk.Text(self.editor_frame, height=5, width=40)
        options_widget.grid(row=row, column=0, columnspan=2, sticky="w")
        options_widget.insert("1.0", options_text)
        self.editor_vars["options"] = options_widget
        row += 1
        return row

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
                entry["duration"] = int(self.editor_vars["duration"].get())
            except ValueError:
                entry["duration"] = 0
        elif t == "Choice":
            prev = entry.get("prevDialogue", {})
            prev["character"] = self.editor_vars["prev_character"].get()
            prev["text"] = self.editor_vars["prev_text"].get()
            entry["prevDialogue"] = prev
            options_str = self.editor_vars["options"].get("1.0", tk.END).strip()
            options = []
            if options_str:
                lines = options_str.splitlines()
                for line in lines:
                    if ": " in line:
                        branch, text = line.split(": ", 1)
                        options.append({"branchId": branch.strip(), "text": text.strip()})
            entry["options"] = options

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

        # 포맷팅된 XML 문자열 생성
        xml_string = ET.tostring(root_elem, encoding="unicode")
        from xml.dom.minidom import parseString
        pretty_xml = parseString(xml_string).toprettyxml(indent="  ")

        # 파일에 저장
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
                            "duration": int(entry_elem.get("Duration", 0))
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
            self.selected_block_index = None
            self.selected_entry_index = None
            self.entry_listbox.delete(0, tk.END)
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
            self.entry_listbox.delete(0, tk.END)
            self.clear_editor()
            messagebox.showinfo("새로 만들기", "새 파일이 생성되었습니다.")

    def on_block_button_press(self, event):
        # Record the starting index of the drag operation
        self.block_drag_start_index = self.block_listbox.nearest(event.y)

    def on_block_button_motion(self, event):
        # Determine the new index as the mouse moves
        current_index = self.block_listbox.nearest(event.y)
        if current_index != self.block_drag_start_index:
            # Swap the blocks in the underlying data list
            self.blocks[self.block_drag_start_index], self.blocks[current_index] = self.blocks[current_index], self.blocks[self.block_drag_start_index]
            # Update the drag start index
            self.block_drag_start_index = current_index
            # Refresh the listbox to reflect changes
            self.refresh_block_list()
            self.block_listbox.select_clear(0, tk.END)
            self.block_listbox.select_set(current_index)

    def on_block_button_release(self, event):
        # Reset the drag start index when mouse button is released
        self.block_drag_start_index = None

# ===== 메인 함수 =====
def main():
    root = tk.Tk()
    app = StoryEditorApp(root)
    root.geometry("1000x600")
    root.mainloop()

if __name__ == "__main__":
    main()