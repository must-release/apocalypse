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

        # 내부 데이터: StoryBlocks 리스트
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
        # 좌측: 블록 리스트
        left_frame = tk.Frame(self)
        left_frame.pack(side=tk.LEFT, fill=tk.Y, padx=5, pady=5)

        tk.Label(left_frame, text="스토리 블록").pack()
        self.block_listbox = tk.Listbox(left_frame, width=30, height=20)
        self.block_listbox.pack(fill=tk.Y, expand=True)
        self.block_listbox.bind("<<ListboxSelect>>", self.on_block_select)

        # 블록 순서 변경 버튼
        block_button_frame = tk.Frame(left_frame)
        block_button_frame.pack(fill=tk.X, pady=5)
        tk.Button(block_button_frame, text="▲", command=self.move_block_up).pack(side=tk.LEFT, padx=2)
        tk.Button(block_button_frame, text="▼", command=self.move_block_down).pack(side=tk.LEFT, padx=2)

        # 블록 추가 버튼
        tk.Button(left_frame, text="블록 추가", command=self.add_block).pack(fill=tk.X, pady=5)

        # 중앙: 엔트리 리스트
        center_frame = tk.Frame(self)
        center_frame.pack(side=tk.LEFT, fill=tk.BOTH, expand=True, padx=5, pady=5)

        tk.Label(center_frame, text="엔트리 리스트").pack()
        self.entry_listbox = tk.Listbox(center_frame, width=40, height=20)
        self.entry_listbox.pack(fill=tk.BOTH, expand=True)
        self.entry_listbox.bind("<<ListboxSelect>>", self.on_entry_select)

        # 엔트리 추가 버튼
        entry_button_frame = tk.Frame(center_frame)
        entry_button_frame.pack(fill=tk.X, pady=5)
        self.entry_type_var = tk.StringVar(value="Dialogue")
        entry_type_dropdown = ttk.Combobox(
            entry_button_frame, textvariable=self.entry_type_var, state="readonly",
            values=["Dialogue", "Effect", "Choice"]
        )
        entry_type_dropdown.pack(side=tk.LEFT, padx=2)
        tk.Button(entry_button_frame, text="엔트리 추가", command=self.add_entry_to_block).pack(side=tk.LEFT, padx=2)

        # 우측: 편집 패널
        right_frame = tk.Frame(self)
        right_frame.pack(side=tk.RIGHT, fill=tk.BOTH, expand=True, padx=5, pady=5)
        tk.Label(right_frame, text="편집").pack(anchor="w")
        self.editor_frame = tk.Frame(right_frame)
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

    def move_block_up(self):
        if self.selected_block_index is not None and self.selected_block_index > 0:
            idx = self.selected_block_index
            self.blocks[idx], self.blocks[idx - 1] = self.blocks[idx - 1], self.blocks[idx]
            self.refresh_block_list()
            self.block_listbox.select_set(idx - 1)
            self.on_block_select(None)

    def move_block_down(self):
        if self.selected_block_index is not None and self.selected_block_index < len(self.blocks) - 1:
            idx = self.selected_block_index
            self.blocks[idx], self.blocks[idx + 1] = self.blocks[idx + 1], self.blocks[idx]
            self.refresh_block_list()
            self.block_listbox.select_set(idx + 1)
            self.on_block_select(None)

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
            new_entry = {"type": "Dialogue", "character": "", "text": ""}
        elif entry_type == "Effect":
            new_entry = {"type": "Effect", "action": "", "duration": 0}
        elif entry_type == "Choice":
            new_entry = {"type": "Choice", "prevDialogue": {"character": "", "text": ""}, "options": []}
        else:
            return

        self.blocks[self.selected_block_index]["entries"].append(new_entry)
        self.refresh_entry_list()

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
        t = entry["type"]

        tk.Label(self.editor_frame, text=f"편집 - {t}").grid(row=0, column=0, columnspan=2, sticky="w")
        row = 1

        self.editor_vars = {}

        if t == "Dialogue":
            tk.Label(self.editor_frame, text="캐릭터:").grid(row=row, column=0, sticky="e")
            char_var = tk.StringVar(value=entry.get("character", ""))
            tk.Entry(self.editor_frame, textvariable=char_var).grid(row=row, column=1, sticky="w")
            self.editor_vars["character"] = char_var
            row += 1

            tk.Label(self.editor_frame, text="대사:").grid(row=row, column=0, sticky="e")
            text_var = tk.StringVar(value=entry.get("text", ""))
            tk.Entry(self.editor_frame, textvariable=text_var, width=40).grid(row=row, column=1, sticky="w")
            self.editor_vars["text"] = text_var
            row += 1

        # Add similar logic for "Effect" and "Choice" types...

        tk.Button(self.editor_frame, text="변경 저장", command=self.save_editor_changes).grid(row=row, column=0, columnspan=2, pady=5)

    def save_editor_changes(self):
        if self.selected_block_index is None or self.selected_entry_index is None:
            return

        block = self.blocks[self.selected_block_index]
        entry = block["entries"][self.selected_entry_index]
        t = entry["type"]

        if t == "Dialogue":
            entry["character"] = self.editor_vars["character"].get()
            entry["text"] = self.editor_vars["text"].get()

        # Add similar logic for "Effect" and "Choice" types...

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

# ===== 메인 함수 =====
def main():
    root = tk.Tk()
    app = StoryEditorApp(root)
    root.geometry("1000x600")
    root.mainloop()

if __name__ == "__main__":
    main()
