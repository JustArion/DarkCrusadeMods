OUT_DIR = ..\\Shared
OBJ_FILE = $(OUT_DIR)\CLRBootstrap.obj

# /EHsc = C4530: C++ exception handler used, but unwind semantics are not enabled.
# /c 	= No build
# /Z6	= Built in .pdb debug symbols
# /std	= Language Standard (Use C++ 20)
CXXFLAGS = /EHs /Z7 /c /std:c++20

cl = C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Tools\MSVC\14.42.34433\bin\Hostx86\x86\cl.exe

rebuild:
	$(cl) $(CXXFLAGS) dllmain.cpp /Fo"$(OBJ_FILE)"
#cl $(CXXFLAGS) dllmain.cpp /Fo"$(OBJ_FILE)"

build: $(OBJ_FILE)

$(OBJ_FILE): dllmain.cpp | $(OUT_DIR)

	cl $(CXXFLAGS) dllmain.cpp /Fo"$(OBJ_FILE)"

$(OUT_DIR):
	mkdir $(OUT_DIR)

help:
	$(info Usage: make <target>)
	$(info )
	$(info Targets:)
	$(info  build       Build the Core Runtime Bootstrapper)
	$(info  rebuild     Build without caching)
	$(info  help        Show this help message)
	@:
