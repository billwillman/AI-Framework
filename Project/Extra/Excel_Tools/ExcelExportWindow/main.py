# This Python file uses the following encoding: utf-8
import sys
from PySide6.QtWidgets import QApplication
from QExportExcelMainUI import *


if __name__ == "__main__":
    app = QApplication(sys.argv)
    # ...
    window = QExportExcelMainUI()
    window.show()
    sys.exit(app.exec())
