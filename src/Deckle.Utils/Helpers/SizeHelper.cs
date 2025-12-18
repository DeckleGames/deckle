using System;
using System.Collections.Generic;
using System.Text;

namespace Deckle.Utils.Helpers;

public static class SizeHelper
{
    const decimal _mmPerInch = 25.4m;

    public static int MmToPx(decimal Mm, int Dpi) => (int)Math.Round((Mm / _mmPerInch) * Dpi);
}
