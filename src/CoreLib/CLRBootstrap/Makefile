SHELL := cmd.exe
.SHELLFLAGS := /C

OUT_DIR = ..\\..\\Shared
OBJ_FILE = $(OUT_DIR)\CLRBootstrap.obj
VCVARS = "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvarsall.bat" x86

# /EHsc = C4530: C++ exception handler used, but unwind semantics are not enabled.
# /c 	= No build
# /Z6	= Built in .pdb debug symbols
# /std	= Language Standard (Use C++ 20)
CXXFLAGS = /EHsc /Z7 /c /std:c++20

rebuild:
	@call $(VCVARS) && cl $(CXXFLAGS) dllmain.cpp /Fo"$(OBJ_FILE)"

build: $(OBJ_FILE)

$(OBJ_FILE): dllmain.cpp | $(OUT_DIR)
	@call $(VCVARS) && cl $(CXXFLAGS) dllmain.cpp /Fo"$(OBJ_FILE)"

$(OUT_DIR):
	mkdir $(OUT_DIR)

print-includes:
	@call $(VCVARS) && echo INCLUDE = %INCLUDE%

help:
	$(info Usage: make <target>)
	$(info )
	$(info Targets:)
	$(info  build       Build the .NET Runtime Bootstrapper)
	$(info  rebuild     Build without caching)
	$(info  help        Show this help message)
	@:
