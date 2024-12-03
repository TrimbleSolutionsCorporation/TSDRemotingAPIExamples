from pythonnet import load
load("coreclr")

import clr
import sys
import time

# Add the directory where your DLLs are located
project_dir = r'.\bin\Debug\net8.0'  # Adjust as needed
sys.path.append(project_dir)

# Load the .NET assembly
clr.AddReference("System")
clr.AddReference("TSD.API.Remoting")

# Import the .NET namespace and types
import TSD.API.Remoting as TSD
from System.Collections.Generic import List

# Connect to TSD instance and start interacting with API to get the model
instance = TSD.ApplicationFactory.GetFirstRunningApplicationAsync().Result
isConnected = instance.Connected
document = instance.GetDocumentAsync().Result
model = document.GetModelAsync().Result

# Get the name of the entity based on the entity type and index
def get_entity_name(entity_type, index):
    index_dotnet_list = List[int]()
    index_dotnet_list.Add(index)

    switcher = {
        TSD.Common.EntityType.Connection: model.GetConnectionsAsync(index_dotnet_list).Result,
        TSD.Common.EntityType.Member: model.GetMembersAsync(index_dotnet_list).Result,
    }

    response = switcher.get(entity_type, None)

    if response is None:
        return f"Entity type {entity_type} is not supported"

    return list(response)[0].Name

# Define event handler for selection change event in TSD
def selection_event_handler(sender, event_args):
    selection = model.GetSelectedEntitiesAsync().Result

    filtered = []
    filtered.extend(filter( (lambda item: hasattr(item, 'Entity') ), selection ))

    for item in filtered:
        itemName = get_entity_name(item.Entity.Type, item.Entity.Index)
        print(f'Selected item: {item.Entity.Type}, Index: {item.Entity.Index}, Name: {itemName}')

instance.SelectionChanged += selection_event_handler

# Print the TSD data
print('Is Connected: ', isConnected)
print('TSD version: ', instance.GetVersionStringAsync().Result)
print('Model Id: ', document.ModelId)

while True:
    time.sleep(1)
    # read terminal input if "exit" then close
    if input() == "exit":
        break
