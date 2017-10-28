using System;

/*******************************************************
 * Programmed by
 *			 syed Faraz mahmood
 *			 	Student NU FAST ICS
 * can be reached at s_faraz_mahmood@hotmail.com
 * 
 * 
 * 
 * *******************************************************/
	/// <summary>
	/// test driver for the MD5
	/// </summary>
	public class MainClass
	{
		public MainClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		public static void Main ()
		{
			
           char i;
			bool wantexit =false;

			
				try
				{
					Console.WriteLine("Asalamo Alikum \n This Program is a driver program for MD5 algo that i have implented");
					Console.WriteLine("Please select a mode for Demostration");

						
					while (!wantexit)
					{
		
					Console.WriteLine("1)Console");
					Console.WriteLine("2)Windows");
					Console.WriteLine("3)exit");
					Console.Write("So what do u want 1/2/3>");
					i=(char)Console.Read();

						if (i=='1')
						{
							MD5.MD5 md ;
							string tstr;
							md= new MD5.MD5();

							Console.WriteLine("Enter ur string to see what it hashes to");
							Console.WriteLine("To quit the program Enter \"thank you faraz\" as new value ");
							Console.WriteLine("Dont forget to mail me about ur Comments on the MD5 class");
							Console.WriteLine("here i am s_faraz_mahmood@hotmail.com");

							Console.WriteLine("");Console.WriteLine("");
							Console.Write("New Value>");
							tstr=Console.ReadLine();
							while (tstr.ToUpper() !="THANK YOU FARAZ")
							{
								md.Value=tstr;
								Console.WriteLine("MD5 Value =" +md.FingerPrint);
								Console.Write("New Value>");
								tstr=Console.ReadLine();
							}

							wantexit=true;

						}
						else if (i=='2')
						{
							FrmDriver fd = new FrmDriver();
							//fd.Show();
							fd.ShowDialog();
							
							wantexit=true;	
					
						}
						else if (i=='3')
						{
							wantexit=true;
						}

						else wantexit= false;

				}

				
				
			 }
			catch (Exception ex)
			{
				Console.WriteLine("error {0}",ex.Message);
			}

			finally
			{
				Console.WriteLine("press enter to continue");
				Console.Read();
			}
		}
	}

