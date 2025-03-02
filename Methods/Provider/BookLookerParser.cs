using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using Buchdatenbank.UserControls;

namespace Buchdatenbank;

// Klasse zum verarbeiten der Informationen über das Buch, das
// von Booklooker stammt. Hier werden die Preise (min) und die 
// weiteren Informationen wie z.B. Autor, Titel, Verlag, Jahr etc.
// weiter verarbeitet bzw. herausgeholt. 
internal class BookLookerParser
{
    static string[]? splittenEuro;
    //internal static Books bookInfo = new Books();
    private static readonly HtmlDocument docHtml = new();
    private static readonly string pathNodeMain = @"/html/body/div/div[4]/main/section";
    //private static readonly string pathNodeFirst = @"/html/body/div[3]/div[4]/main/section/div[2]/div[2]/div";
    //static int number = 1;
    internal static int counterBooklooker = 0;
    //private static readonly List<Books> booksBl = new List<Books>();
    //static Books? blBooks;
    static string node = "";
    

    #region Holen der Webseite (Main)
    internal static void HtmlBooklookerParserMain(string HtmlCode, Books bookBL)
    {
        _ = new Books();
        // Holen des Html-Codes für die anschliessende Untersuchung und Filterung.
        docHtml.LoadHtml(HtmlCode);

        // Kontrolle ob das Buch angeboten wird.
        try
        {
            string nodePath = pathNodeMain + "/table[2]";
            node = docHtml.DocumentNode.SelectSingleNode(nodePath).OuterHtml;

            if (node != null)
            {
                OfferOrNotOffer.PassBookToOffer(true);
                BuchDetails.blNoOffer = true;
                counterBooklooker++;

                // Herausholen des Preises
                string nodePrice = docHtml.DocumentNode.SelectSingleNode("//tr/td[3]/div/div/span").InnerText;
                splittenEuro = nodePrice.Split('&');
                bookBL.Price = splittenEuro[0];

                //Console.WriteLine("Preis ermitteln.... ");
                //Console.WriteLine("Preis (Booklooker): {0} € ", splittenEuro);

                // Herausholen der Adresse für die weiteren Informationen über das Buch.
                var PathToInfo = docHtml.DocumentNode.SelectSingleNode("//tr[1]/td[3]/div/span/a").GetAttributeValue("href", "");
                GetInformationBook(PathToInfo, bookBL);
            }
            else
            {
                OfferOrNotOffer.PassBookToOffer(false);
                BuchDetails.blNoOffer = false;
            }
        }
        catch
        {
           
        }
    }
    #endregion

    #region Detail-Informationen über das Buch
    // Methode zum filtern der benötigten Detail-Informationen über das Buch.
    internal static string GetInformationBook(string PathInfo, Books book)
    {
        string MainPathInfo = "https://www.booklooker.de/" + PathInfo;
        string pathNodeInformation = @"/div";

        // Holen der Detail-Informationen
        string infoWebsite = HttpClientHtml.GetHtmlCode(MainPathInfo).Result;
        docHtml.LoadHtml(infoWebsite);

        try
        {
            string PathForDetailInformation = pathNodeMain + pathNodeInformation;

            // Zähler für die Div-Elemente, damit es nicht zu einer Out of Bounds Exception kommt.                
            int counterChilds = docHtml.DocumentNode.SelectNodes(PathForDetailInformation).Count;
            //string nodeInnerDiv = "";

            // Hier werden die Elemente die die Informationen beinhalten herausgeholt.
            HtmlNode getElements = docHtml.DocumentNode.SelectSingleNode(PathForDetailInformation);

            IEnumerable<HtmlNode> nodes = getElements.Elements("div");

            // Methode für eine Null-Reference Exception, wenn eine gegeben wird wird sie 
            // hier abgefangen.
            nodes = HelperMethod(PathForDetailInformation, getElements, nodes);

            InfoToBooks(nodes, book);
        }
        catch (Exception e)
        {
            /* Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Exception Caught!");
            Console.WriteLine("Message :{0}", e.Message);
            Console.ResetColor(); */
            return e.Message;
        }
        return node;
    }
    #endregion
    // Anzeigen der Informationen in der Konsole zur Überprüfung, wird später wieder
    // entfernt. TODO
    private static void InfoToBooks(IEnumerable<HtmlNode> nodes, Books book)
    {
        //string? nodeInnerDiv = "";

        foreach (HtmlNode node1 in nodes)
        {
            if (node1.NodeType == HtmlNodeType.Element)
            {
                if (node1.GetAttributeValue("class", "") != null)
                {
                    switch (node1.GetAttributeValue("class", ""))
                    {
                        case "propertyItem_2":
                            book.Author = StringRegexDecode.DecodeHtml(node1.SelectSingleNode("div[2]").InnerText).Replace("\n", "");
                            break;
                        case "propertyItem_1":
                            book.Title = StringRegexDecode.DecodeHtml(node1.SelectSingleNode("div[2]").InnerText);
                            break;
                        case "propertyItem_6":
                            //book.Isbn10 = StringRegexDecode.DecodeHtml(node1.SelectSingleNode("div[2]").InnerText);
                            break;
                        case "propertyItem_9":
                            book.Publisher = StringRegexDecode.DecodeHtml(node1.SelectSingleNode("div[2]").InnerText).Replace("\n", "");
                            break;
                        case "propertyItem_18":
                            book.Published = StringRegexDecode.DecodeHtml(node1.SelectSingleNode("div[2]").InnerText.Replace(".", ""));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        // TODO nur für den Test erforderlich!!!!
        //Console.ForegroundColor = ConsoleColor.Yellow;
        //Console.Write("{0}", htmlDecode.Replace("\t", String.Empty));
        //Console.ResetColor();

        //// Hier werden noch überflüssige Tabulatoren entfernt.
        //HtmlStreamWriter.WriteToFileHtml(htmlDecode.Replace("\t", string.Empty), number);
        //number++;
        /*
        try
        {
            blBooks = new Books
            {
                Author = docHtml.DocumentNode.SelectSingleNode("//div[contains(@class, 'propertyItem_2')]/div[2]").InnerText,
                Title = docHtml.DocumentNode.SelectSingleNode("//div[contains(@class, 'propertyItem_1')]/div[2]").InnerText,
                Isbn = docHtml.DocumentNode.SelectSingleNode("//div[contains(@class, 'propertyItem_6')]/div[2]").InnerText,
                Publisher = docHtml.DocumentNode.SelectSingleNode("//div[contains(@class, 'propertyItem_9')]/div[2]").InnerText,
                //Listed_at = docHtml.DocumentNode.SelectSingleNode("//div[contains(@class, 'propertyItem_18')]/div[2]").InnerText
            };
        }
        catch
        {
            System.Console.WriteLine("Fehler!");
        }
        // Hinzufügen zu einer Liste, da mehrere Bücher verarbeitet werden sollen!
        booksBl.Add(blBooks);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine("Autor: {0}", blBooks.Author);
        Console.WriteLine("Titel: {0}", blBooks.Title);
        Console.WriteLine("ISBN: {0}", blBooks.Isbn);
        Console.WriteLine("Verlag: {0}", blBooks.Publisher);
        Console.WriteLine("Erschienen: {0}", blBooks.Listed_at);
        Console.ResetColor();*/
    }

    // Für den Fall das der Verkäufer zurzeit nicht anwesend ist oder sonstige Informationen
    // Sonst gibt es eine Null-Reference Exception, und die Detail-Informationen werden nicht
    // übermittelt.
    static IEnumerable<HtmlNode> HelperMethod(string info, HtmlNode getElements, IEnumerable<HtmlNode> nodes)
    {
        if (!getElements.Elements("div").Any())
        {
            getElements = docHtml.DocumentNode.SelectSingleNode(info + "/div[2]");
            nodes = getElements.Elements("div");
        }
        else
        {
            /*Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" -Wichtige Info für den Grabscher- ");
            Console.ResetColor();*/
            getElements = docHtml.DocumentNode.SelectSingleNode(info + "/div[2]");
            nodes = getElements.Elements("div");
        }
        return nodes;
    }
}
