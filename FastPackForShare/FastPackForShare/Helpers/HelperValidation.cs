using FastPackForShare.Extensions;
using System.Net.Mail;

namespace FastPackForShare.Helpers;

public static class HelperValidation
{
    public static bool ItensList()
    {
        List<string> arrNames = new List<string>() { "xpto", "Visual C#", null, ".NET", "2017" };
        bool result = arrNames.TrueForAll(val => !string.IsNullOrEmpty(val));
        return result;
    }

    public static bool Cpf(string vrCPF)
    {
        bool equal = true;
        string value = vrCPF.Replace(".", "").Replace("-", "");

        if (value.Length != 11)
            return false;

        for (int i = 1; i < 11 && equal; i++)

            if (value[i] != value[0])

                equal = false;

        if (equal || value == "12345678909")
            return false;

        int[] numbers = new int[11];

        for (int i = 0; i < 11; i++)

            numbers[i] = int.Parse(value[i].ToString());

        int sum = 0;

        for (int i = 0; i < 9; i++)

            sum += (10 - i) * numbers[i];

        int result = sum % 11;

        if (result == 1 || result == 0)
        {
            if (numbers[9] != 0)
                return false;
        }

        else if (numbers[9] != 11 - result)
            return false;

        sum = 0;

        for (int i = 0; i < 10; i++)

            sum += (11 - i) * numbers[i];

        result = sum % 11;

        if (result == 1 || result == 0)
        {
            if (numbers[10] != 0)
                return false;
        }

        else
            if (numbers[10] != 11 - result)
            return false;

        return true;

    }

    public static bool Cnpj(string vrCNPJ)
    {

        string CNPJ = vrCNPJ.Replace(".", "").Replace("/", "").Replace("-", "");

        int[] digits, sum, result;

        int nrDig;

        string ftmt;

        bool[] CNPJOk;

        ftmt = "6543298765432";

        digits = new int[14];

        sum = new int[2];

        sum[0] = 0;

        sum[1] = 0;

        result = new int[2];

        result[0] = 0;

        result[1] = 0;

        CNPJOk = new bool[2];

        CNPJOk[0] = false;

        CNPJOk[1] = false;


        for (nrDig = 0; nrDig < 14; nrDig++)
        {

            digits[nrDig] = int.Parse(

                CNPJ.Substring(nrDig, 1));

            if (nrDig <= 11)

                sum[0] += digits[nrDig] *

                  int.Parse(ftmt.Substring(

                  nrDig + 1, 1));

            if (nrDig <= 12)

                sum[1] += digits[nrDig] *

                  int.Parse(ftmt.Substring(

                  nrDig, 1));

        }

        for (nrDig = 0; nrDig < 2; nrDig++)
        {

            result[nrDig] = sum[nrDig] % 11;

            if (result[nrDig] == 0 ||

                 result[nrDig] == 1)

                CNPJOk[nrDig] =

                digits[12 + nrDig] == 0;

            else

                CNPJOk[nrDig] =

                digits[12 + nrDig] ==

                11 - result[nrDig];

        }

        return CNPJOk[0] && CNPJOk[1];
    }

    public static bool Email(string email)
    {
        bool isEmailValid = false;

        string emailRegex = string.Format("{0}{1}",
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))",
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");

        return isEmailValid = Regex.IsMatch(email, emailRegex);
    }

    public static bool TelAndCel(string digitNumbers)
    {
        string[] arrDigitNumbersMovel = new string[] { "6", "7", "8", "9" };
        string[] arrDigitNumbersFix = new string[] { "1", "2", "3", "4", "5" };

        if (GuardClauseExtension.IsNullOrWhiteSpace(digitNumbers) == false && digitNumbers.Length > 5)
        {
            var digitNumbersMovel = string.Join(",", arrDigitNumbersMovel);
            var digitNumbersFix = string.Join(",", arrDigitNumbersFix);

            if (digitNumbers.Length < 13 && digitNumbersMovel.Contains(digitNumbers.ApplySubString(5, 1)))
                return true;

            else if (digitNumbers.Length < 14 && digitNumbersFix.Contains(digitNumbers.ApplySubString(5, 1)))
                return true;
        }

        return false;
    }

    public static bool Url(string baseUrl, string url)
    {
        Uri baseUri = new Uri(baseUrl);
        Uri uriAddress = new Uri(url);
        return baseUri.IsBaseOf(uriAddress);
    }

    public static bool FullName(string fullName)
    {
        var arrName = fullName.ApplyTrim().Split(' ');

        if (GuardClauseExtension.IsNullOrWhiteSpace(fullName) || arrName.Length == 1 || arrName.Where(x => x.Replace(".", string.Empty).Length == 1).Count() > 0)
            return false;

        return true;
    }

    public static bool CNH(string cnhDigits)
    {
        bool isValid = false;

        if (cnhDigits.Length == 11 && cnhDigits != new string('1', 11))
        {
            var dsc = 0;
            var v = 0;
            for (int i = 0, j = 9; i < 9; i++, j--)
            {
                v += Convert.ToInt32(cnhDigits[i].ToString()) * j;
            }

            var vl1 = v % 11;
            if (vl1 >= 10)
            {
                vl1 = 0;
                dsc = 2;
            }

            v = 0;
            for (int i = 0, j = 1; i < 9; ++i, ++j)
            {
                v += Convert.ToInt32(cnhDigits[i].ToString()) * j;
            }

            var x = v % 11;
            var vl2 = x >= 10 ? 0 : x - dsc;

            isValid = vl1.ToString() + vl2.ToString() == cnhDigits.Substring(cnhDigits.Length - 2, 2);
        }

        return isValid;
    }

    public static bool CousinNumberByLINQ(int numCousin)
    {
        return Enumerable.Range(2, (int)Math.Sqrt(numCousin) - 1).All(divider => numCousin % divider != 0);
    }

    public static bool CousinNumber(int numCousin)
    {
        bool numPrimo = true;

        for (int divider = 2; divider <= Math.Sqrt(numCousin); divider++)
        {
            if (numCousin % divider == 0)
            {
                numPrimo = false;
                break;
            }
        }

        return numPrimo;
    }

    public static bool EMailAddress(string mailAddress)
    {
        return MailAddress.TryCreate(mailAddress, out var _);
    }

    public static bool CheckIpAddressIsValid(string ipAddress)
    {
        IPAddress IP;
        bool flag = IPAddress.TryParse(ipAddress, out IP);
        return flag;
    }
}
