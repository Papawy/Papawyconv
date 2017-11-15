
Papawyconv
===

### Version 0.1.6
### Under Apache Licence 2.0

Install
---

Simply put the exe in a directory. It is much easier if you put it in your working directory.

How to use
---

Use : `papawyconv.exe --options`

A basic use with an IPL file and an IDE file would be :
`papawyconv.exe --ide file.ide --ipl file.ipl`

```
-v, --verbose    Print details during execution.
--ide            (Default: ) The IDE file to convert.
--art            (Default: artconfig.txt) The artconf file that will be generated. Need an IDE file.
--app            (Default: False) Append lines in the artconf generated file from an IDE conversion.
--ipl            (Default: ) The IPL file to convert. Work better with an IDE conversion in the same execution.
--pwn            (Default: streamer.pwn) The pawn file that will be generated. Need an IPL file.
--acc            (Default: False) For an IPL conversion, accept objects without custom SAMP ID.
--nolod          (Default: False) Remove LOD objects, they are sometimes no needed.
--dir            (Default: ) Set a custom directory for models during an IDE conversion.
--addx           (Default: 0) Add x units to all objects positions. Usefull for entire maps placement. Only during an IPL conversion.
--addy           (Default: 0) Add y units to all objects positions. Usefull for entire maps placement. Only during an IPL conversion.
--addz           (Default: 0) Add z units to all objects positions. Usefull for entire maps placement. Only during an IPL conversion.
--drawdist       (Default: -1) Set a drawdistance for all objects. Take effect during an IPL conversion.
--streamdist     (Default: -1) Set a streamdistance for all objects. Take effect during an IPL conversion.
--help           Display this help screen.
```
I recommend to always put an IDE file during an IPL conversion because it pass some arguments like the new SAMP ID generated in the artconfig.txt .

Build
---

It is built with Visual Studio 2017 and with .Net 4.5.
It uses [CommandLineParser ](https://github.com/gsscoder/commandline).
It uses ILMerge for merging assemblies.

Questions
---

- My generated pawn file contain only positive objects IDs !

That's mostly because you forgot to link an IDE file while converting.

Thanks
---
RIDE2DAY for bug testing.