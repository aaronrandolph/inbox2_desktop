using System;
using System.Collections.Generic;
using System.Linq;
using Inbox2.Core.Search.Reflection;
using Inbox2.Framework.Interfaces;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Version = Lucene.Net.Util.Version;

namespace Inbox2.Core.Search
{
	static class SearchHelper
	{
		/// <summary>
		/// Creates a document from the given object by analyzing its decorated properties.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance">The instance.</param>
		/// <param name="mappers"></param>
		/// <returns></returns>
		public static Document CreateDocument<T>(T instance, params IContentMapper[] mappers)
		{
			// Read properties to store 
			Document doc = new Document();

			foreach (var token in Reflector.GetTokensFrom(instance, mappers))
			{
				doc.Add(
					new Field(token.Name, token.Value, 
					          token.Store ? Field.Store.YES : Field.Store.NO, 
					          token.Tokenize ? Field.Index.ANALYZED : Field.Index.NOT_ANALYZED));
			}

			// Add type marker
            doc.Add(new Field("Type", typeof(T).Name, Field.Store.YES, Field.Index.NOT_ANALYZED));

			return doc;
		}

		public static Query BuildQuery<T>(string searchQuery, Analyzer analyzer)
		{
			var tokens = Reflector.GetTokensFrom(default(T)).ToList();

			string[] fields = new string[tokens.Count + 1];
			string[] queries = new string[tokens.Count +1];			
			Occur[] occurs = new Occur[tokens.Count + 1];

			for (int i = 0; i < tokens.Count; i++)
			{
				var token = tokens[i];

				queries[i] = token.Name;
				fields[i] = searchQuery;
				occurs[i] = Occur.SHOULD;
			}

			queries[tokens.Count] = "Type";
			fields[tokens.Count] = typeof(T).Name;
			occurs[tokens.Count] = Occur.SHOULD;

			return MultiFieldQueryParser.Parse(Version.LUCENE_30, fields, queries, occurs, analyzer);
		}		
	}
}