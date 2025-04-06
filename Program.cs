using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace decodeConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter file path:");
            string filePath = Console.ReadLine();
            //string path = "C:\Games\scryde\ScrydeAuth.ini";
            if (File.Exists(filePath))
            {
                HashSet<string> decodedStrings = new HashSet<string>();
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    string[] lineStriped = null;
                    string[] loginStriped = null;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("Password_"))
                        {
                            lineStriped = line.Split('=');
                        }

                        if (line.Contains("Login"))
                        {
                            loginStriped = line.Split('=');
                        }

                        if (lineStriped != null && !String.IsNullOrEmpty(lineStriped[1]))
                        {
                            byte[] encryptedBytes = null;
                            byte[] decryptedBytes = null;
                            string decrypted = "";

                            try
                            {
                                string base64 = lineStriped[1].Replace("\r", "").Replace("\n", "").Trim().Trim('"');
                                int padding = 4 - (base64.Length % 4);
                                if (padding != 4)
                                {
                                    base64 += new string('=', padding);
                                }

                                encryptedBytes = Convert.FromBase64String(base64);
                                decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
                                decrypted = Encoding.Unicode.GetString(decryptedBytes);
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine("Base64 error: " + ex.Message);
                                try
                                {
                                    encryptedBytes = Convert.FromBase64String(lineStriped[1].Replace("\r", "").Replace("\n", "").Trim().Trim('"'));
                                    decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.LocalMachine);
                                    decrypted = Encoding.Unicode.GetString(decryptedBytes);
                                }
                                catch (Exception innerEx)
                                {
                                    Console.WriteLine("Decription error with LocalMachine: " + innerEx.Message);
                                }
                            }
                            catch (CryptographicException ex)
                            {
                                Console.WriteLine("Decription error: " + ex.Message);
                            }

                            if (!string.IsNullOrEmpty(decrypted) && decodedStrings.Add(decrypted))
                            {
                                Console.WriteLine($"Login: {loginStriped[1]} password: {decrypted.Trim().Replace(" ", "")}");
                            }
                        }
                    }
                }
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("File not found.");
            }
        }
    }
}
