using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Inbox2.Core.Configuration;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace Inbox2.Core.Search
{
	public static class SearchUtil
	{
		public static void InitializeSearchStore()
		{
			string index = Path.Combine(DebugKeys.DefaultDataDirectory, "search");
		    Directory dir = new SimpleFSDirectory(new DirectoryInfo(index));

			if (IndexReader.IndexExists(dir) == false)
			{
				IndexWriter writer = new IndexWriter(dir, new StandardAnalyzer(Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);

				writer.Dispose();
			}
		}
	}
}
