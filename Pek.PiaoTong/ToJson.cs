using System.Collections;
using System.Text;

namespace Pek.PiaoTong;

internal class ToJson
{
    public static String Table2Json(Hashtable table)
    {

        StringBuilder jsonstr = new StringBuilder();
        jsonstr.Append("{");
        foreach (DictionaryEntry tableEntry in table)
        {
            if (tableEntry.Key == "itemList" || tableEntry.Key == "invoiceIssueOptions")
            {
                String liststr = list2json((ArrayList)tableEntry.Value);

                jsonstr.Append(string.Format("\"{0}\":{1},", tableEntry.Key, liststr));

            }
            else
            {
                jsonstr.Append(string.Format("\"{0}\":\"{1}\",", tableEntry.Key, tableEntry.Value));
            }

        }


        jsonstr.Append("}");
        jsonstr.Remove(jsonstr.Length - 2, 1);
        return jsonstr.ToString();
        //            Console.Write(jsonstr.ToString());
    }

    private static String list2json(ArrayList list)
    {
        StringBuilder jsonstr = new StringBuilder();
        jsonstr.Append("[");
        foreach (Hashtable valuelist in list)
        {
            jsonstr.Append(Table2Json(valuelist));
            jsonstr.Append(",");
        }
        jsonstr.Remove(jsonstr.Length - 1, 1);
        jsonstr.Append("]");

        return jsonstr.ToString();
    }

}