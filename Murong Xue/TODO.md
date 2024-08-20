1. Logging:
	a. [ ] Possibly making a new object when we don't need to?
		- ALL bitewise| operators!!!
		- 1. [ ] LogLevel operator |(LogLevel a, LogMod b)
		- 2. [ ] LogLevel operator |(LogLevel a, LogLevel b)
		- 3. [ ] LogLevel operator |(LogLevel a, LogType b)
	b. [ ] search about using Action<Log>() for buffer? Saw online
2. Download Handler + Config?
	a. record batch settings (mintime / size)
	b. Create an algorithm that allows rewards size+/- & time +/-
3. FeedData
	a. Better handling for invalid URLs
	b. [X] Compare dates & history?
4. DownloadEntry
	a. [ ] in downloadentryFile.HandleDownload,
		- The prints statements that show link & path are redundant. No extra information is given by having both