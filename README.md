# BSPMapDeobfuscator
Program made to deobfuscate TopHattWaffle's and Omnicoder's BSP map Obfuscator

The original program modifies the BSP file (used in Source Engine Games) by identifying and marking certain surfaces that have a specific flag called SURF_NOPORTAL. It then modifies some indices in the file to reference those surfaces differently.

This program will simply reverse the process.

# Usage

`csc BSPMapDeobfuscator.cs`

`BSPMapDeobfuscator.exe mapname_proc.bsp` where mapname_proc.bsp is a obfuscated bsp map.

Add '_proc' at the end of the bsp file before processing it.
