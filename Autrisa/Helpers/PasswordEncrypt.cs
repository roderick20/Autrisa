namespace Autrisa.Helper
{
    public class PasswordEncrypt
    {
        public static string Encrypt(string txt_plano)
        {
            System.Security.Cryptography.HashAlgorithm obj_hash = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] cadena_plana = System.Text.Encoding.UTF8.GetBytes(txt_plano);
            byte[] cadena_encrp = obj_hash.ComputeHash(cadena_plana);
            obj_hash.Clear();
            return (Convert.ToBase64String(cadena_encrp));
        }
    }
}