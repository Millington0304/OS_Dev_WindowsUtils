# OS_Dev_WindowsUtils

This is an archived project in C#.NET.

## Maker
Maker does the following:

### Main Function - Build Kernel
1. Takes a list of ASM files and assembley into .o object files (defaultly use NASM)
2. Takes a list of C and CPP files and compile into .o object files (defaultly use GCC/G++)
3. Use GNU linker to link .o object file into a .tmp file (with symbols stripped, etc.)
4. Use GNU Binutils objcopy to make the temporary .tmp file into binary codes and therefore ready to be directly written into a VHD file

## VHD Writer
It writes binary files into a virtual hard drive (VHD) file at the correct location (e.g. cylinder, sector) with necessary modifications (e.g. 0x55aa) for later booting in VMs.
