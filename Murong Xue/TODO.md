1. Logging:
	a. [ ] Possibly making a new object when we don't need to?
		- ALL bitewise| operators!!!
		- 1. [ ] LogLevel operator |(LogLevel a, LogMod b)
		- 2. [ ] LogLevel operator |(LogLevel a, LogLevel b)
		- 3. [ ] LogLevel operator |(LogLevel a, LogType b)
	b. [ ] search about using Action<Log>() for buffer? Saw online
2. Download Handler
	a. record batch settings (mintime / size)
	b. Create an algorithm that allows rewards size+/- & time +/-
	[ ] c. Make Processing[] a local method variable rather than a class var.
		- Thinking something along the lines  of a task that returns an enum or bool
		- The loop would then handle the processing list.
		- The breaking change is that we use Processing to not stop before all files are processed.
		- To avoid this, we add a status value to each Feed? When we are performing the
		- final update to the feeds we check its value & wait for it to finish?
		- Maybe this could be a Task variable the class stores that we attach at the
		- end of OnDownload()? Then when we foreach (feed : feeds) await feed.Handle
		- This leaves the files that would've been added up in the air....
		- Each feed own its files, and maybe its task is actually a List<Task>[]
		- so we can call WhenAll on it? but this would handle requeuing...
3. FeedData
	a. Better handling for invalid URLs
	b. [X] Compare dates & history?
4. DownloadEntry
	a. [ ] in downloadentryFile.HandleDownload,
		- The prints statements that show link & path are redundant. No extra information is given by having both