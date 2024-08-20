	1. Logging:
	a.[ ] interactive mode
		[X] Suppress all Console output that isn't flagged with INTERACTIVE | NORMAL
		[X] Prints only show content, no identifiers or log levels (maybe change ERROR & whatnot?)
		[X] Give a visually distinct marker to avoid hardcoding prefixes?
			[X] - >msg
		[X] {firstCharIdentifier/Acronym}: msg
			?- "CFG: Interact Mode!!!"
			?- "C: ..."
			- FOR EACH REPORTER (4c/5c/<6c>)
				[x] 1. Config				- "CONF" / "CONFG" / "CONFIG"
				[x] 2. DownloadHandler		- "HTTP" / "DOWNL" / "DLHAND"
				[x] 3. DownloadEntryBase	- "ENTB" / "B-ENT" / "DLBASE"
				[x] 4. DownloadEntryFile	- "FILE" / "FLENT" / "DLFILE"
				[x] 5. DownloadEntryFeed	- "FEED" / "FEENT" / "DLFEED"
				[X] 6. FeedData				- "FEED" / "FDATA" / "F-DATA"
				[x] 7. Program				- "PROG"|"MAIN"    / "PROGRM"
				[x] 8. Logger				- "LOGG" / "LOGGR" / "LOGGER"
				[X] 9. Entry Data			- "EDAT" / "EDATA" / "ENTDAT" //ugly
			    [x] 10. Event Ticker		- "EVNT" / "EVENT" / "EVENTT" //ugly
			    [X] 11. Interactive Editor	- "EDIT" / "IEDIT" / "EDITOR"
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
	b. [X] Compare dates & history?
4. DownloadEntry
	a. [ ] in downloadentryFile.HandleDownload,
		- The prints statements that show link & path are redundant. No extra information is given by having both