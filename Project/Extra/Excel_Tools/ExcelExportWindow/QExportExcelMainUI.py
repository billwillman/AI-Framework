# This Python file uses the following encoding: utf-8
from PySide6 import QtCore
from PySide6 import QtWidgets
from PyQt6 import uic
import os


class QExportExcelMainUI(QtWidgets.QMainWindow):
    def __init__(self):
        super().__init__()
        # 加载 .ui 的设计文件
        current_dir = os.path.dirname(os.path.abspath(__file__))
        ui_file_path = os.path.join(current_dir, "mainwindow.ui")
        uic.loadUi(ui_file_path, self)
        pass
