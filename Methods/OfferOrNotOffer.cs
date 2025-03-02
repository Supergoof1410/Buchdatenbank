using System.Windows;
using Buchdatenbank.UserControls;

namespace Buchdatenbank;

public class OfferOrNotOffer
{
    internal static bool yesOrNo;

    internal static void PassBookToOffer(bool isOffer)
    {
        if (isOffer)
        {
            yesOrNo = true;
        }
        else
        {
            yesOrNo = false;
        }
    }
}
