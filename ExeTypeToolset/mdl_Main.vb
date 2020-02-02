Module mdl_Main
	Public opt_7z_Location As String = ".\7z\7z.exe"
	Public opt_SearchPath As String = ""
	Public opt_TempPath As String = Alphaleonis.Win32.Filesystem.Path.GetTempPath
	Public opt_Outfile As String = "results.csv"
	Public opt_7zExtensions As String = ".zip .rar .7z .cue .iso"

	Public archiveExtensions As String = ""

	Public opt_moveDOSpath As String = ""
	Public opt_moveNONDOSpath As String = ""
	Public opt_moveERRORpath As String = ""
	Public opt_moveDirectory As Boolean = False
	Public opt_moveDryRun As Boolean = False

	Public Class cls_Counter
		Public directory As String
		Public file As String
		Public dos As Integer = 0
		Public nondos As Integer = 0
		Public errors As Integer = 0

		Public Sub New(ByVal directory As String, ByVal file As String)
			Me.directory = directory
			Me.file = file
		End Sub
	End Class

	Public Enum enm_ExeType
		dos = 0
		nondos = 1
		erroneous = 2
	End Enum

	Sub Main()
		Dim bmissingSearchPath As Boolean = True
		Dim bShowHelpAndQuit As Boolean = False

		Console.WriteLine("ExeTypeToolset v1.1 by MK2k")
		Console.WriteLine()

		For Each arg As String In Environment.GetCommandLineArgs
			If arg.StartsWith("-7z=") Then
				opt_7z_Location = arg.Split("=")(1).Replace("""", "")
			ElseIf arg.StartsWith("-7zextensions=") Then
				opt_7zExtensions = arg.Split("=")(1).Replace("""", "")
			ElseIf arg.StartsWith("-i=") Then
				opt_SearchPath = arg.Split("=")(1).Replace("""", "")
			ElseIf arg.StartsWith("-o=") Then
				opt_Outfile = arg.Split("=")(1).Replace("""", "")
			ElseIf arg.StartsWith("-t=") Then
				opt_TempPath = arg.Split("=")(1).Replace("""", "")
			ElseIf arg.StartsWith("-movedos=") Then
				opt_moveDOSpath = arg.Split("=")(1).Replace("""", "")
			ElseIf arg.StartsWith("-movenondos=") Then
				opt_moveNONDOSpath = arg.Split("=")(1).Replace("""", "")
			ElseIf arg.StartsWith("-moveerror=") Then
				opt_moveERRORpath = arg.Split("=")(1).Replace("""", "")
			ElseIf arg = "-movedirectory" Then
				opt_moveDirectory = True
			ElseIf arg = "-movedryrun" Then
				opt_moveDryRun = True
			ElseIf arg = "-h" Then
				bShowHelpAndQuit = True
			End If
		Next

		Console.WriteLine("Environment")
		Console.WriteLine("-----------")
		Console.WriteLine()
		Console.WriteLine("7z location     : " & opt_7z_Location)
		Console.WriteLine("Output File     : " & opt_Outfile)
		Console.WriteLine("Input Path      : " & opt_SearchPath)

		If opt_moveDOSpath <> "" Then Console.WriteLine("move DOS path   : " & opt_moveDOSpath)
		If opt_moveNONDOSpath <> "" Then Console.WriteLine("move NONDOS path: " & opt_moveNONDOSpath)
		If opt_moveERRORpath <> "" Then Console.WriteLine("move ERROR path : " & opt_moveNONDOSpath)
		If opt_moveDirectory Then Console.WriteLine("[X] Move directory instead of single files")
		If opt_moveDirectory Then Console.WriteLine("[X] DRY RUN mode")

		Console.WriteLine()

		If Not Alphaleonis.Win32.Filesystem.File.Exists(opt_7z_Location) Then
			Console.WriteLine("ERROR: 7z location could not be found: '" & opt_7z_Location & "'")
			bShowHelpAndQuit = True
		Else
			Dim formatsdir = Alphaleonis.Win32.Filesystem.Path.GetDirectoryName(opt_7z_Location) & "\" & "Formats"
			If Not Alphaleonis.Win32.Filesystem.Directory.Exists(formatsdir) Then
				Console.WriteLine("ERROR: the 7z installation is missing the 'Formats' subdirectory (download and install Iso7z from http://www.tc4shell.com/en/7zip/iso7z/)")
				bShowHelpAndQuit = True
			Else
				If Not Alphaleonis.Win32.Filesystem.File.Exists(formatsdir & "\Iso7z.64.dll") AndAlso Not Alphaleonis.Win32.Filesystem.File.Exists(formatsdir & "\Iso7z.32.dll") Then
					Console.WriteLine("ERROR: the 7z installation is missing either Iso7z.64.dll or Iso7z.32.dll (download and install Iso7z from http://www.tc4shell.com/en/7zip/iso7z/)")
					bShowHelpAndQuit = True
				End If
			End If
		End If

		If opt_7zExtensions.Trim = "" Then
			Console.WriteLine("ERROR: the 7z extensions provided by -7zextensions seem to be empty")
			bShowHelpAndQuit = True
		Else
			For Each ext As String In opt_7zExtensions.Split(" ")
				If Not ext.StartsWith(".") Then
					Console.WriteLine("ERROR: extension '" & ext & "' in your list of 7z extensions does not start with a '.'")
					bShowHelpAndQuit = True
				End If
			Next
		End If

		If Not Alphaleonis.Win32.Filesystem.File.Exists(".\dosidcli.exe") Then
			Console.WriteLine("ERROR: .\dosidcli.exe is missing")
			bShowHelpAndQuit = True
		End If

		If opt_SearchPath = "" Then
			Console.WriteLine("ERROR: input path is missing")
			bShowHelpAndQuit = True
		ElseIf Not Alphaleonis.Win32.Filesystem.Directory.Exists(opt_SearchPath) Then
			Console.WriteLine("ERROR: input path '" & opt_SearchPath & "' not found!")
			bShowHelpAndQuit = True
		End If

		If Not Alphaleonis.Win32.Filesystem.Directory.Exists(opt_TempPath) Then
			Console.WriteLine("ERROR: temp path '" & opt_TempPath & "' not found!")
			bShowHelpAndQuit = True
		End If

		If opt_moveDOSpath <> "" AndAlso Not Alphaleonis.Win32.Filesystem.Directory.Exists(opt_moveDOSpath) Then
			Console.WriteLine("ERROR: path for moving DOS releases '" & opt_moveDOSpath & "' not found!")
			bShowHelpAndQuit = True
		End If

		If opt_moveNONDOSpath <> "" AndAlso Not Alphaleonis.Win32.Filesystem.Directory.Exists(opt_moveNONDOSpath) Then
			Console.WriteLine("ERROR: path for moving non-DOS releases '" & opt_moveNONDOSpath & "' not found!")
			bShowHelpAndQuit = True
		End If

		If opt_moveERRORpath <> "" AndAlso Not Alphaleonis.Win32.Filesystem.Directory.Exists(opt_moveERRORpath) Then
			Console.WriteLine("ERROR: path for moving ERROR releases '" & opt_moveERRORpath & "' not found!")
			bShowHelpAndQuit = True
		End If

		If bShowHelpAndQuit Then
			Console.WriteLine()
			Console.WriteLine("Usage:")
			Console.WriteLine()
			Console.WriteLine("-i={input path}      - location of (sub)directories containing to-be-analyzed releases (.zip/.7z/.cue+bin/.iso)")
			Console.WriteLine("-o={output file}     - (optional) location of output .csv file (default: 'results.csv')")
			Console.WriteLine("-t={temp path}       - (optional) location for temporary, extracted, files (default: system-defined temp)")
			Console.WriteLine("-7z={7z location}    - (optional) location of your 7z installation including iso7z (default: '.\7z\7z.exe')")
			Console.WriteLine("-7zextensions={list} - (optional) list of extensions you want to handle via 7z (default: '.zip .rar .7z .cue .iso')")
			Console.WriteLine("-movedos={path}      - move DOS releases to this path (optional)")
			Console.WriteLine("-movenondos={path}   - move non-DOS releases to this path (optional)")
			Console.WriteLine("-moveerror={path}    - move releases containing errors to this path (optional)")
			Console.WriteLine("-movedirectory       - instead of single files, move the whole directory (use this if a release is always contained in its own directory)")
			Console.WriteLine("-movedryrun          - don't actually move anything just mention it in the console")
			Console.WriteLine("-h                   - show this help")
			Console.WriteLine()
			Console.WriteLine("examples:")
			Console.WriteLine("most simple : ExeTypeToolset.exe -i=""g:\My ISO Collection\Redump""")
			Console.WriteLine("more complex: ExeTypeToolset.exe -i=""g:\My ISO Collection\Redump"" -o=""redump.csv"" -7z=""d:\Apps\7Zip\7z.exe"" -t=""d:\Temp"" -7zextensions="".zip .rar .7z .cue .bin"" -movedos=""g:\My ISO Collection\Redump\_DOS_"" -movenondos=""g:\My ISO Collection\Redump\_NON_DOS_"" -moveerror=""g:\My ISO Collection\Redump\_ERROR_""")
			Console.WriteLine()
			Console.WriteLine("Move rules:")
			Console.WriteLine("1. A release is treated as ERROR if at least one executable could not be identified as either DOS or non-DOS")
			Console.WriteLine("2. A release is treated as DOS if at least one executable is identified as DOS")
			Console.WriteLine("3. A release is treated as non-DOS if at least one executable is identified as non-DOS and no executable is identified as DOS")
			Return
		End If

		GenerateArchiveExtensions()

		Try
			Alphaleonis.Win32.Filesystem.File.Delete(opt_Outfile)
		Catch ex As Exception

		End Try

		If Not Alphaleonis.Win32.Filesystem.File.Exists("dosidcli.exe") Then
			Console.WriteLine("dosidcli.exe missing!")
			Return
		End If

		AppendTextToFile(opt_Outfile, "DIRECTORY	FILE	DOS	NONDOS	ERRORS")

		AnalyzeDirectory(opt_SearchPath, 0)

#If DEBUG Then
		Console.WriteLine(ControlChars.CrLf & "DONE, please press a key")
		Console.ReadKey()
#End If
	End Sub

	Public Function CreateTempDir() As String
		Dim sTemp As String = ""

		Dim iCounter As Integer = 0
		Do
			sTemp = opt_TempPath & "\efi_" & "tmp_" & DateTime.Now.ToString("yyyyMMddHHmmssfff") & IIf(iCounter > 0, iCounter, "")
			If Not Alphaleonis.Win32.Filesystem.Directory.Exists(sTemp) Then
				Exit Do
			End If
			iCounter += 1
		Loop

		If Alphaleonis.Win32.Filesystem.Directory.Exists(sTemp) Then

		End If

		Try
			Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(sTemp)
		Catch ex As Exception
			Throw ex
		End Try

		If Alphaleonis.Win32.Filesystem.Directory.Exists(sTemp) Then
			Return sTemp
		Else
			Throw New Exception("Error creating temporary directory!")
		End If

		Return ""

	End Function

	Public Sub GenerateArchiveExtensions()
		archiveExtensions = "*.exe *.bin"

		For Each ext As String In opt_7zExtensions.Split(" ")
			archiveExtensions &= " *" & ext.ToLower
		Next
	End Sub

	Public Function GetIndent(level) As String
		Dim indent As String = ""
		For i = 0 To level - 1
			indent &= "  "
		Next
		Return indent
	End Function

	Public Sub AnalyzeDirectory(ByVal cwd As String, ByVal level As Integer, Optional ByRef counter As cls_Counter = Nothing)
		If counter Is Nothing Then Console.WriteLine(GetIndent(level) & "analyzing " & cwd.Replace(opt_SearchPath, ""))

		Dim files As String() = Alphaleonis.Win32.Filesystem.Directory.GetFiles(cwd)
		Dim directories As String() = Alphaleonis.Win32.Filesystem.Directory.GetDirectories(cwd)

		If files.Length = 0 Then
			If counter Is Nothing Then Console.WriteLine("  no files found")
		Else
			For Each file As String In files
				AnalyzeFile(file, level + 1, counter)
			Next
		End If

		For Each directory As String In directories
			AnalyzeDirectory(directory, level + 1, counter)
		Next
	End Sub

	Public Sub AnalyzeFile(ByVal fullpath As String, ByVal level As Integer, ByRef counter As cls_Counter)
		'The file may already be gone - because we move whole directories of releases by their identification
		If Not Alphaleonis.Win32.Filesystem.File.Exists(fullpath) Then
			Return
		End If

		Dim filename As String = Alphaleonis.Win32.Filesystem.Path.GetFileName(fullpath)
		Dim extension As String = Alphaleonis.Win32.Filesystem.Path.GetExtension(fullpath).ToLower

		Console.WriteLine(GetIndent(level) & "analyzing " & filename)

		If opt_7zExtensions.Split(" ").Contains(extension) Then
			AnalyzeArchive(fullpath, level + 1, counter)
			Return
		End If

		If {".exe"}.Contains(extension) Then
			'AnalyzeExeFileWithDOSIDCLI(fullpath, level + 1, counter)
			AnalyzeExeFileWithDOSFINDER(fullpath, level + 1, counter)
			Return
		End If

		If {".bin"}.Contains(extension) Then
			'Skip without notification (we analyze .cue files)
			Return
		End If

		Console.WriteLine(GetIndent(level + 1) & "-> SKIPPING (unknown filetype)")
	End Sub

	Public Sub AnalyzeArchive(fullpath As String, level As Integer, Optional ByRef counter As cls_Counter = Nothing)
		Console.WriteLine(GetIndent(level) & "-> scanning archive")

		Dim isOriginal As Boolean = False
		If counter Is Nothing Then
			isOriginal = True
			counter = New cls_Counter(Alphaleonis.Win32.Filesystem.Path.GetDirectoryName(fullpath), Alphaleonis.Win32.Filesystem.Path.GetFileName(fullpath))
		End If

		Dim TempDir As String = CreateTempDir()

		Try
			Using process As New Process()
				process.StartInfo.FileName = opt_7z_Location
				process.StartInfo.Arguments = "x -aoa """ & fullpath & """ -o""" & TempDir & """ " & archiveExtensions & " -r"
				process.StartInfo.UseShellExecute = False
				process.StartInfo.RedirectStandardOutput = True
				process.Start()

				' Synchronously read the standard output of the spawned process. 
				Dim procReader As System.IO.StreamReader = process.StandardOutput
				Dim output As String = procReader.ReadToEnd()

				process.WaitForExit()

				If ExtractionHasErrors(output) Then
					counter.errors += 1
				End If

				AnalyzeDirectory(TempDir, level + 1, counter)

				If isOriginal Then
					'counter contains all the info we need about the ISO file, write it down
					AppendTextToFile(opt_Outfile, counter.directory & "	" & counter.file & "	" & counter.dos & "	" & counter.nondos & "	" & counter.errors)
					MoveOriginalFiles(counter, level)
					counter = Nothing
				End If
			End Using
		Catch ex As Exception
			Console.WriteLine(GetIndent(level + 1) & "ERROR: " & ex.Message)
		End Try

		MKNetLib.cls_MKFileSupport.Delete_Directory(TempDir)
	End Sub

	Public Enum enm_MoveType
		Undecided = 0
		Err = 1
		DOS = 2
		NONDOS = 3
	End Enum

	Public Sub MoveOriginalFiles(ByRef counter As cls_Counter, ByVal level As Integer)
		If opt_moveDOSpath = "" AndAlso opt_moveNONDOSpath = "" AndAlso opt_moveERRORpath = "" Then
			Return
		End If

		Console.WriteLine(GetIndent(level) & "Check for MOVING")

		Dim targetPath As String = ""

		If counter.errors > 0 Then
			If opt_moveERRORpath <> "" Then
				targetPath = opt_moveERRORpath
				Console.WriteLine(GetIndent(level) & "release is ERROR, moving " & IIf(opt_moveDirectory, "the WHOLE DIRECTORY", "the individual file/s") & " to '" & opt_moveERRORpath & "' " & IIf(opt_moveDryRun, "(DRY RUN)", ""))
			Else
				Console.WriteLine(GetIndent(level) & "release is ERROR, but no target is specified")
			End If
		ElseIf counter.dos > 0 Then
			If opt_moveDOSpath <> "" Then
				targetPath = opt_moveDOSpath
				Console.WriteLine(GetIndent(level) & "release is DOS, moving " & IIf(opt_moveDirectory, "the WHOLE DIRECTORY", "the individual file/s") & " to '" & opt_moveDOSpath & "' " & IIf(opt_moveDryRun, "(DRY RUN)", ""))
			Else
				Console.WriteLine(GetIndent(level) & "release is DOS, but no target is specified")
			End If
		ElseIf counter.nondos > 0 Then
			If opt_moveNONDOSpath <> "" Then
				targetPath = opt_moveNONDOSpath
				Console.WriteLine(GetIndent(level) & "release is non-DOS, moving " & IIf(opt_moveDirectory, "the WHOLE DIRECTORY", "the individual file/s") & " to '" & opt_moveNONDOSpath & "' " & IIf(opt_moveDryRun, "(DRY RUN)", ""))
			Else
				Console.WriteLine(GetIndent(level) & "release is non-DOS, but no target is specified")
			End If
		End If

		If opt_moveDryRun Then
			Return
		End If

		If opt_moveDirectory Then
			Try
				Dim sourceDir = counter.directory
				Alphaleonis.Win32.Filesystem.Directory.Move(sourceDir, targetPath & "\" & Alphaleonis.Win32.Filesystem.Path.GetDirectoryNameWithoutRoot(sourceDir & "\"))
			Catch ex As Exception
				Console.WriteLine("ERROR: " & ex.Message)
			End Try
		Else
			If Alphaleonis.Win32.Filesystem.Path.GetExtension(counter.file).ToLower = ".cue" Then
				MoveCueBin(counter, targetPath)
			Else
				Try
					Alphaleonis.Win32.Filesystem.File.Move(counter.directory & "\" & counter.file, targetPath & "\" & counter.file)
				Catch ex As Exception

				End Try
			End If
		End If
	End Sub

	Public Sub MoveCueBin(counter As cls_Counter, targetPath As String)
		Dim cueContents As String = MKNetLib.cls_MKFileSupport.GetFileContents(counter.directory & "\" & counter.file)
		Dim arrFiles As New ArrayList

		arrFiles.Add(counter.file) 'we want to move the .cue file itself too

		For Each line As String In cueContents.Split(ControlChars.Lf)
			line = line.Trim
			If line.ToLower.StartsWith("file") Then
				Dim file As String = MKNetLib.cls_MKRegex.GetMatches(line, "\""(.*?)\""")(0).Groups(1).Value
				arrFiles.Add(file) 'add the files referenced in the .cue file to the list
			End If
		Next

		For Each file As String In arrFiles
			Try
				Alphaleonis.Win32.Filesystem.File.Move(counter.directory & "\" & file, targetPath & "\" & file)
			Catch ex As Exception

			End Try
		Next
	End Sub

	Public Function ExtractionHasErrors(output As String) As Boolean
		For Each line As String In output.Split(ControlChars.Lf)
			line = line.Trim
			If line.ToLower.Contains("can't open as archive") Then
				Return True
			End If
		Next

		Return False
	End Function

	Public Sub AnalyzeExeFileWithDOSIDCLI(fullPath As String, level As Integer, ByRef counter As cls_Counter)
		Dim isOriginal = False
		If counter Is Nothing Then
			isOriginal = True
			counter = New cls_Counter("", "")
		End If

		If Not Alphaleonis.Win32.Filesystem.File.Exists(fullPath) Then
			counter.errors += 1
			Return
		End If

		'call dosidcli with fullPath
		Try
			Using process As New Process()
				process.StartInfo.FileName = "dosidcli.exe"
				process.StartInfo.Arguments = """" & fullPath & """"
				process.StartInfo.UseShellExecute = False
				process.StartInfo.RedirectStandardOutput = True
				process.Start()

				' Synchronously read the standard output of the spawned process. 
				Dim procReader As System.IO.StreamReader = process.StandardOutput
				Dim output As String = procReader.ReadToEnd()

				process.WaitForExit()

				If output.Contains("DOS!") Then
					counter.dos += 1
					Console.ForegroundColor = ConsoleColor.Green
					Console.WriteLine(GetIndent(level + 1) & " DOS! yay! \o/")
					Console.ForegroundColor = ConsoleColor.Gray
				ElseIf output.Contains("SOMETHINGELSE!") Then
					counter.nondos += 1
					Console.ForegroundColor = ConsoleColor.Blue
					Console.WriteLine(GetIndent(level + 1) & " non-dos boo! :(")
					Console.ForegroundColor = ConsoleColor.Gray
				Else
					Console.ForegroundColor = ConsoleColor.Red
					Console.WriteLine(GetIndent(level + 1) & " ERROR: not identifiable :o")
					Console.ForegroundColor = ConsoleColor.Gray
					counter.errors += 1
				End If
			End Using
		Catch ex As Exception
			counter.errors += 1

		End Try

		If isOriginal Then
			counter = Nothing
		End If

		Return
	End Sub

	Public Sub AnalyzeExeFileWithDOSFINDER(fullPath As String, level As Integer, ByRef counter As cls_Counter)
		Dim isOriginal = False
		If counter Is Nothing Then
			isOriginal = True
			counter = New cls_Counter("", "")
		End If

		If Not Alphaleonis.Win32.Filesystem.File.Exists(fullPath) Then
			counter.errors += 1
			Return
		End If

		'TODO: Create temp folder
		Dim tempDir As String = CreateTempDir()

		Dim destinationFullPath As String = MKNetLib.cls_MKStringSupport.Clean_Right(tempDir, "\") & "\" & Alphaleonis.Win32.Filesystem.Path.GetFileName(fullPath)



		'call dosidcli with fullPath
		Try
			Alphaleonis.Win32.Filesystem.File.Copy(fullPath, destinationFullPath)

			Using process As New Process()
				process.StartInfo.FileName = ".\dosfinder\dosfinder.exe"
				process.StartInfo.WorkingDirectory = ".\dosfinder\"
				process.StartInfo.Arguments = """" & tempDir & """" & " -p -t -i"
				process.StartInfo.UseShellExecute = False
				process.StartInfo.RedirectStandardOutput = True
				process.Start()

				' Synchronously read the standard output of the spawned process. 
				Dim procReader As System.IO.StreamReader = process.StandardOutput
				Dim output As String = procReader.ReadToEnd()

				process.WaitForExit()

				If output.Contains("MSDOS") OrElse output.Contains("DOS Executable") Then
					counter.dos += 1
					Console.ForegroundColor = ConsoleColor.Green
					Console.WriteLine(GetIndent(level + 1) & " DOS! yay! \o/")
					Console.ForegroundColor = ConsoleColor.Gray
				ElseIf output.Contains("DIE:WIN") Then
					counter.nondos += 1
					Console.ForegroundColor = ConsoleColor.Blue
					Console.WriteLine(GetIndent(level + 1) & " non-dos boo! :(")
					Console.ForegroundColor = ConsoleColor.Gray
				Else
					Console.ForegroundColor = ConsoleColor.Red

					Dim identifier As String = ":o"

					If MKNetLib.cls_MKRegex.IsMatch(output, "[DIE:.*?]") Then
						identifier = " " & MKNetLib.cls_MKRegex.GetMatches(output, "\[.*\]")(0).Value
					End If

					Console.WriteLine(GetIndent(level + 1) & " ERROR: not identifiable :" & identifier)
					Console.ForegroundColor = ConsoleColor.Gray
					counter.errors += 1
				End If
			End Using
		Catch ex As Exception
			counter.errors += 1
		Finally
			Try
				MKNetLib.cls_MKFileSupport.Delete_Directory(tempDir)
			Catch ex As Exception

			End Try
		End Try

		If isOriginal Then
			counter = Nothing
		End If

		Return
	End Sub

	Public Sub AppendTextToFile(filepath, text)
		Using sw As IO.StreamWriter = Alphaleonis.Win32.Filesystem.File.AppendText(filepath)
			sw.WriteLine(text)
		End Using

	End Sub

End Module
