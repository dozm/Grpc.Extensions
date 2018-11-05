"""
根据 proto 文件生成 C# 代码。
"""
import os,sys,platform
import xml.etree.ElementTree as ET
from os import path, system, getcwd, walk

# 将在该目录及其子目录中查找 .proto 文件。
protos_dir = path.join(path.abspath('..'), "protos")
# 指定生成文件的输出目录。
csharp_out_dir = grpc_out_dir = path.join('..', "generated")
# 默认会自动查找，如果需要，你可以指定目录，它应该包含 protoc 和 plugin 工具。
grpc_tools_dir = None


def get_proto_dict(dirname):
    """
    获取指定目录及其子目录下的 .proto 文件。   
    Returns:
        返回一个字典，key 为目录的完整路径，value 为该目录下的 .proto 文件名列表。
    """
    proto_dic = {}

    for dir, subdir, files in walk(dirname):
        file_names = [f for f in files if f.endswith(".proto")]
        if(len(file_names) > 0):
            proto_dic[dir] = file_names

    return proto_dic


def exec_command(cmd):
    print(f"Executing:\n\n{cmd}")
    create_dir(csharp_out_dir)
    create_dir(grpc_out_dir)
    status = system(cmd)
    print(f"\nCommand status: {status}")


def build_command():
    global grpc_tools_dir
    grpc_tools_dir = grpc_tools_dir or get_grpctools_dir()

    protoc,grpc_csharp_plugin = get_grpctools_name()
    protoc = path.join(grpc_tools_dir, protoc)
    grpc_csharp_plugin = path.join(grpc_tools_dir, grpc_csharp_plugin)       
    proto_dic = get_proto_dict(protos_dir)

    import_paths = " ".join(['-I="{}"'.format(key) for key in proto_dic.keys()])
    proto_file_names = " ".join(['"{}"'.format(f) for files in proto_dic.values() for f in files])

    return f'{protoc} {import_paths} {proto_file_names} --csharp_out="{csharp_out_dir}" --grpc_out="{grpc_out_dir}" --plugin=protoc-gen-grpc="{grpc_csharp_plugin}"'

def create_dir(dir):
    if not path.exists(dir):
        os.makedirs(dir)

def get_grpctools_dir():
    """先查找当前目录，再查找 nuget 全局包目录。"""

    grpctools_dir = 'grpc.tools'
    if not path.isdir(grpctools_dir):
         grpctools_dir = path.join(os.path.expanduser('~'),'.nuget','packages',grpctools_dir)
    if not path.isdir(grpctools_dir):
        raise Exception(f"未找到 grpc.tools 目录：{grpctools_dir}。")

    grpctool_version = get_grpctools_version()
    os_platform = platform.system().lower()

    if os_platform == "darwin":
        os_platform = "macosx"

    os_platform = f'{os_platform}_x{platform.architecture()[0][0:2]}'

    return path.join(grpctools_dir,grpctool_version,'tools',os_platform)

def get_grpctools_name():
    protoc = "protoc"
    grpc_csharp_plugin = "grpc_csharp_plugin"

    if(platform.system() == "Windows"):
        protoc+=".exe"
        grpc_csharp_plugin +=".exe"

    return (protoc,grpc_csharp_plugin)

def get_grpctools_version():
    """从 csproj 文件获取 Grpc.Tools 包的版本号"""
    csproj = get_csproj_file()    
    if not csproj:
        raise Exception('未找到 .csproj 文件。')
    grpctools_pkg_ref = ET.ElementTree(file=csproj).getroot().find('ItemGroup//PackageReference[@Include="Grpc.Tools"]')
    if not grpctools_pkg_ref:
        raise Exception(f'项目 {csproj} 未引用 Grpc.Tools 包。')
   
    return grpctools_pkg_ref.attrib['Version']

def get_csproj_file():
    """从当前目录开始，向上查找 csproj 文件"""
    current_dir = '.'
    while current_dir:
       csproj = next((f for f in os.listdir(current_dir) if f.endswith('.csproj')),'')
       if not csproj:
           current_dir = path.join(current_dir,'..')
       else:
           return path.join(current_dir,csproj)
            
       

def run():
    try:
        cmd = build_command()
        exec_command(cmd)
    except Exception as e:
        print("ERROR:{}".format(e))


if __name__ == "__main__":
    run()
    input("Press Enter key to exit.")
