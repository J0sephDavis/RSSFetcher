1. Logging:
	a.[ ] interactive mode
		[X] Suppress all Console output that isn't flagged with INTERACTIVE | NORMAL
		[X] Prints only show content, no identifiers or log levels (maybe change ERROR & whatnot?)
		[X] Give a visually distinct marker to avoid hardcoding prefixes?
			[X] - >msg
		[ ] {firstCharIdentifier/Acronym}: msg
			?- "CFG: Interact Mode!!!"
			?- "C: ..."
	b. [ ] Possibly making a new object when we don't need to?
		- ALL bitewise| operators!!!
		- 1. [ ] LogLevel operator |(LogLevel a, LogMod b)
		- 2. [ ] LogLevel operator |(LogLevel a, LogLevel b)
		- 3. [ ] LogLevel operator |(LogLevel a, LogType b)
	c. [ ] search about using Action<Log>() for buffer? Saw online
2. Download Handler + Config?
	a. record batch settings (mintime / size)
	b. Create an algorithm that allows rewards size+/- & time +/-
3. FeedData
	a. Better handling for invalid URLs
4. DownloadEntry
	a. [ ] in downloadentryFile.HandleDownload,
		- The prints statements that show link & path are redundant. No extra information is given by having both