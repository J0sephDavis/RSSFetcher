//Feed class
// - link to XML resource
// - DownloadXML()
/* From C21.03, format of our local data
 * <xml ...> //HEADER
 * <root>
 * 	<item>
 * 		<title>title of the feed or whatever - this is probably just for us</title>
 * 		<feedFileName>FileName</feedFileName>
 * 		<feed-url><![CDATA[https://examples.com/RSS]]</feed-url>
 * 		<expr>regex here</expre>
 * 		<history>title of the last downloaded rss entry</history>
 * 	</item>
 * 	...ad infinitum
 * </root>
 */
/* Library options for handling XML feeds
 * - XmlDocument (https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmldocument?view=net-8.0)
 *  1. Handles the document In-Memory (more memory used than stream-based solutions)
 *  2. "Represents an XML document. You can use this class to load, validate, edit, add,
 *  and position XML in a document".
 * - XmlReader
 *  1. Stream-based
 *  2. "Represents a reader that provides fast, noncached, forward-only access to XML data".
 * - XmlWriter
 *  1. Stream-based
 *  2. "Represents a writer that provides a fast, non-cached, forward-only way to generate streams
 *  or files that contain XML data".
 */
/* Notes on handling XML
 * 1. XMLReader for handling the content feeds
 * 2. XMLDocument for handling our own data
 * 3. I doubt it is of concern, but it will be worth taking into
 *  consideration how much memory is used for an XMLDocument and whether we should
 *   3.1. Use an XML Reader to load data at the beginning & use an XMLDocument at the end to make changes
 *   3.2. Use an XML Document throughout the entire lifetime of the program to manage our data
 *   3.3. Use an XMLReader to load data at the beginning, and have a ToString() option for each FeedData which is used in the XMLWriter
 * 
 */
using Murong_Xue;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace MurongXue;
public class Program
{
    //! path to the rss-config.xml file we store data in
    //TODO make a local path
    //private static readonly Uri filePath
    //    = new Uri("D:\\VisualStudio Community Projects\\Murong Xue\\Murong Xue\\rss-config.xml");
    private static EntryData? RSSEntries = null;
    private static bool KeepRunning = true;
    private static Config cfg = Config.GetInstance();
    public static async Task Main()
    {
        RSSEntries = new EntryData(cfg.GetRSSPath());
        await RSSEntries.Process();
    }
}