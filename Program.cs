using GLFW; //Przestrzeń nazw biblioteki GLFW.NET. Zawiera ona bindingi do biblioteki GLFW zapewniającej możliwość tworzenia aplikacji wykorzystujacych OpenGL
using GlmSharp; //Przestrzeń nazw biblioteki GlmSharp. GlmSharp to port biblioteki GLM - OpenGL Mathematics implementującej podstawowe operacje matematyczne wykorzystywane w grafice 3D.

using Shaders; //Przestrzeń nazw pomocniczej biblioteki do wczytywania programów cieniującch

using System;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

using System.Drawing;



namespace PMLabs
{


    //Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenGL do współpracy z OpenTK.
    public class BC: IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {

        static ShaderProgram sp; //Wskaźnik na obiekt reprezentujący program cieniujący
        static float speed_y; //Prędkość obrotu wokół osi Y [rad/s]        
        static float angle_y = 0; //Aktualny kąt obrotu wokół osi Y

        static KeyCallback kc = KeyProcessor;

        static float time = 0;

        //***********TUTAJ MODYFIKACJE - START

        static int nogi;
        static int korpus;

        //***********TUTAJ MODYFIKACJE - KONIEC 

        //Obsługa klawiatury - zmiana prędkości obrotu wokół poszczególnych osi w zależności od wciśniętych klawiszy
        public static void KeyProcessor(System.IntPtr window, Keys key, int scanCode, InputState state, ModifierKeys mods) { 
            if (state==InputState.Press)
            {
                if (key == Keys.Left) speed_y = -3.14f;
                if (key == Keys.Right) speed_y =  3.14f;
             
            }
            if (state == InputState.Release)
            {
                if (key == Keys.Left) speed_y = 0;
                if (key == Keys.Right) speed_y = 0;             
            }
        }

        //Metoda wykonywana po zainicjowaniu bibliotek, przed rozpoczęciem pętli głównej
        //Tutaj umieszczamy nasz kod inicjujący
        public static void InitOpenGLProgram(Window window)
        {
            GL.ClearColor(0, 0, 0, 1); //Wyczyść zawartość okna na czarno (r=0,g=0,b=0,a=1)
            sp = new ShaderProgram("vs.glsl", "fs.glsl"); //Wczytaj przykładowy program cieniujący
            Glfw.SetKeyCallback(window, kc); //Zarejestruj metodę obsługi klawiatury
            GL.Enable(EnableCap.DepthTest);
            //***********TUTAJ MODYFIKACJE - START

            nogi = ReadTexture("leg.png");
            korpus = ReadTexture("corpus.png");

            //***********TUTAJ MODYFIKACJE - KONIEC

        }

        //Metoda wykonywana po zakończeniu pętli główej, przed zwolnieniem zasobów bibliotek
        //Tutaj zwalniamy wszystkie zasoby zaalokowane na począdku programu
        public static void FreeOpenGLProgram(Window window)
        {
            //***********TUTAJ MODYFIKACJE - START

            GL.DeleteTexture(nogi);
            GL.DeleteTexture(korpus);

            //***********TUTAJ MODYFIKACJE - KONIEC
        }

        

        //Wczytuje plik z teksturą i kopiuje teksturę do pamięciu karty graficznej. Zwraca uchwyt na wczytaną teksturę
        public static int ReadTexture(string filename)
        {
            var tex = GL.GenTexture(); //Wygeneruj nowy uchwyt
            GL.ActiveTexture(TextureUnit.Texture0); //Aktywuj zerową jednostkę teksturującą...
            GL.BindTexture(TextureTarget.Texture2D, tex);//... i powiąż ją z nowym uchwytem

            Bitmap bitmap = new Bitmap(filename); //Wczytaj plik z obrazkiem 
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits( //Utwórz bufor w pamięci z obrazkiem
              new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
              System.Drawing.Imaging.ImageLockMode.ReadOnly,
              System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, //Kopiuj obrazek do pamięci karty graficznej
              data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
             
            bitmap.UnlockBits(data); //Zwolnij bufor w pamięci komputera
            bitmap.Dispose();//Zwolnij pamięć zajętą przez obrazek

            //Ustaw teksturowanie z uzyciem bilinear filtering
            GL.TexParameter(TextureTarget.Texture2D,
              TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
              TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);


             
            //Zwróć uchwyt na wczytaną teksturę
            return tex;
        }

        static void DrawCube(mat4 M, bool corpus,float a, float b, float c)
        {

            mat4 M1 = M * mat4.Scale(a / 2, b / 2, c / 2);
            GL.UniformMatrix4(sp.U("M"), 1, false, M1.Values1D);

            GL.EnableVertexAttribArray(sp.A("a_v"));
            GL.VertexAttribPointer(sp.A("a_v"), 4, VertexAttribPointerType.Float, false, 0, MyCube.vertices);

            //***********TUTAJ MODYFIKACJE - START

            GL.EnableVertexAttribArray(sp.A("a_v"));
            GL.VertexAttribPointer(sp.A("a_v"), 4, VertexAttribPointerType.Float, false, 0, MyCube.vertices);

            GL.EnableVertexAttribArray(sp.A("a_n"));
            GL.VertexAttribPointer(sp.A("a_n"), 4, VertexAttribPointerType.Float, false, 0, MyCube.normals); 

            GL.EnableVertexAttribArray(sp.A("a_t"));
            GL.VertexAttribPointer(sp.A("a_t"),
            2, VertexAttribPointerType.Float,
            false, 0, MyCube.texCoords);

            if (corpus == false)
            {
                GL.Uniform1(sp.U("texunit"), 5);
                GL.ActiveTexture(TextureUnit.Texture5);
                GL.BindTexture(TextureTarget.Texture2D, nogi);
            }
            else
            {
                GL.Uniform1(sp.U("texunit"), 5);
                GL.ActiveTexture(TextureUnit.Texture5);
                GL.BindTexture(TextureTarget.Texture2D, korpus);
            }
       
            //***********TUTAJ MODYFIKACJE - KONIEC

            GL.DrawArrays(PrimitiveType.Triangles, 0, MyCube.vertexCount);

            GL.DisableVertexAttribArray(sp.A("a_v"));

            //***********TUTAJ MODYFIKACJE - START

            GL.DisableVertexAttribArray(sp.A("a_t")); 

            GL.DisableVertexAttribArray(sp.A("a_n"));

            //***********TUTAJ MODYFIKACJE - KONIEC
        }


        //Metoda wykonywana najczęściej jak się da. Umieszczamy tutaj kod rysujący
        public static void DrawScene(Window window)
        {
            // Wyczyść zawartość okna (buforów kolorów i głębokości)
            GL.Clear(ClearBufferMask.ColorBufferBit| ClearBufferMask.DepthBufferBit);

            mat4 P = mat4.Perspective(glm.Radians(50.0f), 1, 1, 50); //Wylicz macierz rzutowania
            mat4 V = mat4.LookAt(new vec3(0, 1, -2), new vec3(0, 0, 0), new vec3(0, 1, 0)); //Wylicz macierz widoku

            sp.Use();//Aktywuj program cieniujący
            GL.UniformMatrix4(sp.U("P"), 1, false, P.Values1D); //Wyślij do zmiennej jednorodnej P programu cieniującego wartość zmiennej P zadeklarowanej powyżej
            GL.UniformMatrix4(sp.U("V"), 1, false, V.Values1D); //Wyślij do zmiennej jednorodnej V programu cieniującego wartość zmiennej V zadeklarowanej powyżej


            float light_x = 2 * (float)Math.Cos(time);
            float light_y = 2;
            float light_z = -2 ;
            float light_w = 1;

            //***********TUTAJ MODYFIKACJE - START

            GL.Uniform4(sp.U("zm"), light_x, light_y, light_z, light_w);

            //***********TUTAJ MODYFIKACJE - KONIEC

            float stepangle = 0.5f * (float)Math.Sin(2*time);
            

            mat4 M = mat4.Rotate(angle_y, new vec3(0, 1, 0));

            mat4 MBody = M;
            DrawCube(MBody, true, 0.5f, 0.12f, 0.25f);

            mat4 MLeg1 = M * mat4.Translate(new vec3(0.25f - 0.05f, -0.125f + 0.12f, 0.125f + 0.05f))*mat4.Rotate(-stepangle,new vec3(0.0f,0.0f,1.0f))* mat4.Translate(new vec3(0,-0.12f,0));
            DrawCube(MLeg1, false, 0.1f, 0.25f, 0.1f);

            mat4 MLeg2 = M * mat4.Translate(new vec3(-(0.25f - 0.05f), -0.125f + 0.12f, 0.125f + 0.05f)) * mat4.Rotate(stepangle, new vec3(0.0f, 0.0f, 1.0f)) * mat4.Translate(new vec3(0, -0.12f, 0));
            DrawCube(MLeg2, false, 0.1f, 0.25f, 0.1f);

            mat4 MLeg3 = M * mat4.Translate(new vec3(0.25f - 0.05f, -0.125f + 0.12f, -0.125f - 0.05f)) * mat4.Rotate(stepangle, new vec3(0.0f, 0.0f, 1.0f)) * mat4.Translate(new vec3(0, -0.12f, 0));
            DrawCube(MLeg3, false, 0.1f, 0.25f, 0.1f);

            mat4 MLeg4 = M * mat4.Translate(new vec3(-(0.25f - 0.05f), -0.125f + 0.12f, -0.125f - 0.05f)) * mat4.Rotate(-stepangle, new vec3(0.0f, 0.0f, 1.0f)) * mat4.Translate(new vec3(0, -0.12f, 0));
            DrawCube(MLeg4, false, 0.1f, 0.25f, 0.1f);

            //Skopiuj ukryty bufor do bufora widocznego            
            Glfw.SwapBuffers(window);
        }

        

        //Metoda główna
        static void Main(string[] args)
        {
            Glfw.Init();//Zainicjuj bibliotekę GLFW

            Window window = Glfw.CreateWindow(500, 500, "OpenGL", GLFW.Monitor.None, Window.None); //Utwórz okno o wymiarach 500x500 i tytule "OpenGL"

            Glfw.MakeContextCurrent(window); //Ustaw okno jako aktualny kontekst OpenGL - tutaj będą realizowane polecenia OpenGL
            Glfw.SwapInterval(1); //Skopiowanie tylnego bufora na przedni ma się rozpocząć po zakończeniu aktualnego odświerzania ekranu

            GL.LoadBindings(new BC()); //Pozyskaj adresy implementacji poszczególnych procedur OpenGL

            InitOpenGLProgram(window); //Wykonaj metodę inicjującą Twoje zasoby 
            
            Glfw.Time = 0; //Wyzeruj licznik czasu

            while (!Glfw.WindowShouldClose(window)) //Wykonuj tak długo, dopóki użytkownik nie zamknie okna
            {
                time += (float)Glfw.Time;
                
                angle_y += speed_y * (float)Glfw.Time; //Aktualizuj kat obrotu wokół osi Y zgodnie z prędkością obrotu
                Glfw.Time = 0; //Wyzeruj licznik czasu
                DrawScene(window); //Wykonaj metodę odświeżającą zawartość okna
               
                Glfw.PollEvents(); //Obsłuż zdarzenia użytkownika
            }

            FreeOpenGLProgram(window);//Zwolnij zaalokowane przez siebie zasoby
            

            Glfw.Terminate(); //Zwolnij zasoby biblioteki GLFW
        }
                    

    }
}