# Tekla Structural Designer API from Python

This is a short guide for an experiment using TSD API from Python.
The goal is to connect to running TSD Application and demonstrate we can retrieve some data from it.
The focus here is on all the path finding and dependencies needed in order to get this working.
The author of this guide has a very limited knowledge of Python.

## Introduction

Tekla Structural Designer (TSD) provides a nuget package TeklaStructuralDesigner.RemotingAPI which is intended to be used from a .NET environment.
In order to use this from Python, we need to use a .NET bridge. This means we will need .NET SDK to download the TeklaStructuralDesigner.RemotingAPI nuget package
and its dependencies and then use the bridge to access the API from Python. To demonstrate this is possible I've prepared a simple example this mixed PythonNetExample project.

## Prerequisites
- Tekla Structural Designer installed
- Visual Studio Code
- .NET SDK 8.0 or later
- Python 3

## Steps

1. Install .NET SDK
2. Install PythonNet
3. Build the C# donor project
4. Run TSD open a model and run the Python script

### Install .NET SDK
This can be done in Visual Studio Code via the **.NET Install Tool** extension. Just search for it in the extensions and install it.

### Install PythonNet
This can be done via pip:
```bash
pip install pythonnet
```

### Build the C# donor project
This can be done from command line just navigate to the directory where the PythonNetExperiment project is extracted and run:

```bash
dotnet build
```
This will download the TeklaStructuralDesigner.RemotingAPI nuget package and its dependencies. You will be able to find all the dlls in the project target subfolder.
This is from where pythonnet will load the .NET assemblies. You can see that when the script sets `project_dir` variable.

```bash
<path_to_extracted_location>\PythonNetExperiment\bin\Debug\net8.0
```

### Run TSD open a model and run the Python script
Open Tekla Structural Designer and open any model. Then run the Python script from the extracted location. This will connect to the running TSD application and retrieve some data from it.

```bash
python PythonNetExperiment.py
```
The expected output looks similar to this one, given some selection in the TSD 3D view was made by the user:

```bash
$ python PythonNetExperiment.py

Event listening start!
Is Connected:  True
TSD version:  24.2.0.29
Model Id:  890404f0-3646-4ddc-b8b9-3712ebc99997
Selected item: Member, Index: 94, Name: SB Level 2/10/19-Level 2/10/20
Selected item: Connection, Index: 48, Name: SCC 10/19
Selected item: Member, Index: 77, Name: SGP 10/19
Selected item: ConstructionHelper, Index: 16, Name: Entity type ConstructionHelper is not supported

exit
```

## Conclusion
This proves that it is possible to access the TSD API from Python using PythonNet. This is a simple example and there are many more possibilities to explore.
The current example blocks and waits synchronously for TSD API to finish its work. This example also does not build any conversions between .NET and Python collections it copies the values explicitly.

It is important to take away that a C# project referencing the TeklaStructuralDesigner.RemotingAPI nuget package is needed to act as a donor for the Python script.
The C# project can otherwise be left empty.
