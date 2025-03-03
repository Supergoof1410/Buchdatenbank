using HtmlAgilityPack;
using System.Collections.Generic;
using Buchdatenbank.UserControls;

namespace Buchdatenbank;

internal class ZVABParser
{
    
    private static readonly HtmlDocument docHtml = new();
    internal static int counterZVAB = 0;
    private static readonly string pathNodeFirst = @"/html/body/div[2]/main/div[6]/div[1]/div[2]/ul/li[1]";
    private static readonly string pathForPrice = @"/html/body/div[2]/main/div[6]/div[1]/div[2]/ul/li[1]/div[2]/div[1]/div[2]/div[1]/p";
    private static string node = "";

    #region Holen der Webseite (Main)
    internal static void HtmlZVABParserMain(string HtmlCode, Books bookZvab)
    {

        // Holen des Html-Codes für die anschliessende Untersuchung und Filterung.
        docHtml.LoadHtml(HtmlCode);
        
        try
        {
            node = docHtml.DocumentNode.SelectSingleNode(pathNodeFirst).OuterHtml;

            // Falls es ein Angebot gibt!
            if (node != null)
            {
                OfferOrNotOffer.PassBookToOffer(true);
                BuchDetails.zvabNoOffer = true;
                counterZVAB++;
                BookInformation(bookZvab);
                
            }
            else
            {
                // Falls es kein Angebot gibt!
                OfferOrNotOffer.PassBookToOffer(false);
                BuchDetails.zvabNoOffer = false;
            }
        }
        catch
        {
            
        }
    }
    #endregion

    #region Detailinformationen
    internal static void BookInformation(Books book)
    {
        string nodeInnerDiv = "";

        // Preis ermitteln
        string nodePrice = docHtml.DocumentNode.SelectSingleNode(pathForPrice).InnerText;
        if (nodePrice!.Contains("EUR"))
            book.Price = nodePrice.Remove(0, 3).Trim();
        else book.Price = nodePrice;

        // Hier werden die Elemente die die Informationen beinhalten herausgeholt.
        HtmlNode getElements = docHtml.DocumentNode.SelectSingleNode(pathNodeFirst);

        IEnumerable<HtmlNode> nodes = getElements.Elements("meta");


        foreach (HtmlNode node1 in nodes)
        {
            if (node1.NodeType == HtmlNodeType.Element)
            {
                if (node1.GetAttributeValue("itemprop", "") != null)
                {
                    switch (node1.GetAttributeValue("itemprop", ""))
                    {
                        case "isbn":
                            nodeInnerDiv += "ISBN: " + node1.GetAttributeValue("content", "") + "\n";
                            break;
                        case "name":
                            book.Title = StringRegexDecode.DecodeHtml(node1.GetAttributeValue("content", ""));
                            break;
                        case "author":
                            book.Author = StringRegexDecode.DecodeHtml(node1.GetAttributeValue("content", "")).Replace(":", "");
                            break;
                        case "publisher":
                            book.Publisher = StringRegexDecode.DecodeHtml(node1.GetAttributeValue("content", ""));
                            break;
                        case "datePublished":
                            book.Published = StringRegexDecode.DecodeHtml(node1.GetAttributeValue("content", ""));
                            break;
                        case "bookFormat":
                            if (node1.GetAttributeValue("content", "") == "Hardcover")
                            {
                                book.Cover = 1;
                            }
                            else if (node1.GetAttributeValue("content", "") == "Paperback")
                            {
                                book.Cover = 2;
                            }
                            else book.Cover = 0;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    #endregion
}
