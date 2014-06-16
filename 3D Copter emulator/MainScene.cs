using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

namespace _3D_Copter_emulator
{
    class MainScene
    {
        public static double[] camPos = { 0, 0, 0 };
        public static double[] camCenter = { 0, 0, 0 };
        public static double[] camTop = { 0, 0, 1.0 };
        public static double camDist = 5, camAngle = 45;
        public static int screenWidth, screenHeight;
        public static CopterModel quadrocopter;

        private const int checkImageWidth = 256;
        private const int checkImageHeight = 256;
        static byte[][][] checkImage;
        static int[] texture = new int[3];

        static void Create3DRect(double wx, double wy, double wz) // structure data: x,y,z, wx, wy, wz
        {
            wx = wx / 2;
            wy = wy / 2;
            wz = wz / 2;
            Gl.glBegin(Gl.GL_QUADS);
            // Front Face
            Gl.glNormal3d(0.0f, 0.0f, 1.0f);
            Gl.glVertex3d(-wx, -wy, wz);
            Gl.glVertex3d(wx, -wy, wz);
            Gl.glVertex3d(wx, wy, wz);
            Gl.glVertex3d(-wx, wy, wz);
            // Back Face
            Gl.glNormal3d(0.0f, 0.0f, -1.0f);
            Gl.glVertex3d(-wx, -wy, -wz);
            Gl.glVertex3d(-wx, wy, -wz);
            Gl.glVertex3d(wx, wy, -wz);
            Gl.glVertex3d(wx, -wy, -wz);
            // Top Face
            Gl.glNormal3d(0.0f, 1.0f, 0.0f);
            Gl.glVertex3d(-wx, wy, -wz);
            Gl.glVertex3d(-wx, wy, wz);
            Gl.glVertex3d(wx, wy, wz);
            Gl.glVertex3d(wx, wy, -wz);
            // Bottom Face
            Gl.glNormal3d(0.0f, -1.0f, 0.0f);
            Gl.glVertex3d(-wx, -wy, -wz);
            Gl.glVertex3d(wx, -wy, -wz);
            Gl.glVertex3d(wx, -wy, wz);
            Gl.glVertex3d(-wx, -wy, wz);
            // Right face
            Gl.glNormal3d(1.0f, 0.0f, 0.0f);
            Gl.glVertex3d(wx, -wy, -wz);
            Gl.glVertex3d(wx, wy, -wz);
            Gl.glVertex3d(wx, wy, wz);
            Gl.glVertex3d(wx, -wy, wz);
            // Left Face
            Gl.glNormal3d(-1.0f, 0.0f, 0.0f);
            Gl.glVertex3d(-wx, -wy, -wz);
            Gl.glVertex3d(-wx, -wy, wz);
            Gl.glVertex3d(-wx, wy, wz);
            Gl.glVertex3d(-wx, wy, -wz);
            Gl.glEnd();
        }

        static void BMPtoArr(String filename)
		{	
			Color col;
			Image img = Image.FromFile(filename);
			Bitmap bm = new Bitmap(img);
			for(int i=0; i<img.Width; i++)
				for(int j=0; j<img.Height; j++)
				{
					col = bm.GetPixel(i, j);
					checkImage[i][j][0] = col.R;
					checkImage[i][j][1] = col.G;
					checkImage[i][j][2] = col.B;
					checkImage[i][j][3] = 255;
				}

		}

        static bool InitGL()										// All setup for opengl goes here
        {
            bool status = false;
            Bitmap[] textureImage = new Bitmap[3];
            Rectangle rectangle = new Rectangle();
            BitmapData bitmapData = new BitmapData();

            Gl.glGenTextures(3, texture);
            textureImage[0] = (Bitmap)Image.FromFile("parket.bmp");
            textureImage[1] = (Bitmap)Image.FromFile("texBricks.bmp");
            textureImage[2] = (Bitmap)Image.FromFile("window.bmp");
            textureImage[0].RotateFlip(RotateFlipType.Rotate90FlipNone);
            textureImage[1].RotateFlip(RotateFlipType.Rotate90FlipNone);
            textureImage[2].RotateFlip(RotateFlipType.Rotate90FlipNone);
            
            if (textureImage[0] != null && textureImage[1] != null && textureImage[2] != null)
            {
                status = true;

                textureImage[0].RotateFlip(RotateFlipType.RotateNoneFlipY);
                rectangle = new Rectangle(0, 0, textureImage[0].Width, textureImage[0].Height);
                bitmapData = textureImage[0].LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[0]);

                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_NEAREST);
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGB, textureImage[0].Width,
                                                      textureImage[0].Height, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
                textureImage[0].UnlockBits(bitmapData);
                textureImage[0].Dispose();

                textureImage[1].RotateFlip(RotateFlipType.RotateNoneFlipY);
                rectangle = new Rectangle(0, 0, textureImage[1].Width, textureImage[1].Height);
                bitmapData = textureImage[1].LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[1]);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_NEAREST);
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGB, textureImage[1].Width,
                                                      textureImage[1].Height, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
                textureImage[1].UnlockBits(bitmapData);
                textureImage[1].Dispose();

                textureImage[2].RotateFlip(RotateFlipType.RotateNoneFlipY);
                rectangle = new Rectangle(0, 0, textureImage[2].Width, textureImage[2].Height);
                bitmapData = textureImage[2].LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[2]);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_NEAREST);
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGB, textureImage[2].Width,
                                                      textureImage[2].Height, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
                textureImage[2].UnlockBits(bitmapData);
                textureImage[2].Dispose();
                /*
                // трава
                Gl.glActiveTextureARB(Gl.GL_TEXTURE0_ARB);
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[0]);
                Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);
                
                // a-map
                Gl.glActiveTextureARB(Gl.GL_TEXTURE1_ARB);
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[1]);
                Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);

                // каменистая тропинка
                Gl.glActiveTextureARB(Gl.GL_TEXTURE2_ARB);
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[2]);
                */
                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_COMBINE_ARB);
                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_COMBINE_RGB_ARB, Gl.GL_INTERPOLATE_ARB);

                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE0_RGB_ARB, Gl.GL_TEXTURE0);
                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_OPERAND0_RGB_ARB, Gl.GL_SRC_COLOR);

                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE1_RGB_ARB, Gl.GL_TEXTURE2);
                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_OPERAND1_RGB_ARB, Gl.GL_SRC_COLOR);

                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_SOURCE2_RGB_ARB, Gl.GL_TEXTURE1);
                Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_OPERAND2_RGB_ARB, Gl.GL_SRC_COLOR);

            }
            
            Gl.glShadeModel(Gl.GL_FLAT);

            /*BMPtoArr("tex1.bmp");
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            /*
            Gl.glGenTextures(1, &texName1);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName1);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_NEAREST);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, checkImageWidth, checkImageHeight,
                0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, checkImage);

            
            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGBA, checkImageWidth, checkImageHeight, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, checkImage);
            

            BMPtoArr("texBricks.bmp");

            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            Gl.glGenTextures(1, &texName2);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName2);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_NEAREST);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, checkImageWidth, checkImageHeight,
                0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, checkImage);

            Bitmap bmp = new Bitmap(256, 256);
            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGBA, checkImageWidth, checkImageHeight, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, bmp);

            BMPtoArr("texWin.bmp");

            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);
            Gl.glGenTextures(1, &texName3);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texName3);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_NEAREST);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, checkImageWidth, checkImageHeight,
                0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, checkImage);

            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGBA, checkImageWidth, checkImageHeight, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, checkImage);
            */
            //glShadeModel(GL_SMOOTH);							// Enable smooth shading
            Gl.glClearColor(0.9f, 0.9f, 0.9f, 0.5f);				// Black background
            Gl.glClearDepth(1.0f);									// Depth buffer setup
            Gl.glEnable(Gl.GL_DEPTH_TEST);							// Enables depth testing
            Gl.glDepthFunc(Gl.GL_LEQUAL);								// The type of depth testing to do
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);	// Really nice perspective calculations

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glLightModelf(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_TRUE);
            Gl.glEnable(Gl.GL_NORMALIZE);
            /*
            Gl.glEnable(Gl.GL_BLEND); //Прозрачность
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glShadeModel(Gl.GL_SMOOTH);

            Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_FASTEST);
            Gl.glEnable(Gl.GL_POLYGON_SMOOTH);

            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_ALPHA_TEST);
            
            */
            Gl.glEnable(Gl.GL_MULTISAMPLE);

            // ----------- Сглаживание -----------
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_BLEND);
            // Сглаживание точек
            Gl.glEnable(Gl.GL_POINT_SMOOTH);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);
            // Сглаживание линий
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);
            // Сглаживание полигонов    
            Gl.glEnable(Gl.GL_POLYGON_SMOOTH);
            Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_NICEST);
            // -----------------------------------

            //glEnable(GL_LIGHT0);

            float[] back_color = { 1, 0, 0, 1 };
            Gl.glMaterialfv(Gl.GL_BACK, Gl.GL_DIFFUSE, back_color);

            float[] LightAmbient = { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] LightDiffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] LightPosition = { 1.0f, 1.0f, 30.0f, 1.0f };

            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, LightAmbient);		// Setup The Ambient Light
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, LightDiffuse);		// Setup The Diffuse Light
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, LightPosition);	// Position The Light
            Gl.glEnable(Gl.GL_LIGHT1);

            return true;
        }

        public static void init(int screenW, int screenH)
        {
            screenWidth = screenW;
            screenHeight = screenH;

            quadrocopter = new CopterModel(4);

            camPos[0] = camPos[1] = camPos[2] = 2;

            checkImage = new byte[checkImageHeight][][];
            for (int i = 0; i < checkImageHeight; i++)
            {
                checkImage[i] = new byte[checkImageWidth][];
                for (int j = 0; j < checkImageWidth; j++)
                    checkImage[i][j] = new byte[4];
            }

                Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            // очитка окна 
            Gl.glClearColor(255, 255, 255, 1);

            // установка порта вывода в соотвествии с размерами элемента anT 
            Gl.glViewport(0, 0, screenWidth, screenHeight);


            // настройка проекции 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45, (float)screenWidth / (float)screenHeight, 0.1, 200);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            // настройка параметров OpenGL для визуализации 
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            InitGL();
        }

        public static void computeCamState()
        {
            camPos[0] = camDist * Math.Cos(camAngle / 57.29) + camCenter[0];
            camPos[1] = camDist * Math.Sin(camAngle / 57.29) + camCenter[1];
            //camPos[2] = camCenter[2];

            camTop[0] = -camDist * Math.Cos(camAngle / 57.29);
            camTop[1] = -camDist * Math.Sin(camAngle / 57.29);
        }

        static void drawWalls(double wx, double wy, double wz, double cellSize)
		{
			float[] front_color = {0,0,0,1};
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, front_color);

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[0]);
            double w = 0.3;
            Gl.glBegin(Gl.GL_QUADS);
            for (double i = 0; i < 30; i += w)
				for(double j=0; j<30; j+=w)
				{
					Gl.glNormal3f( 0.0f, 0.0f, 1.0f);
                    Gl.glTexCoord2d(0.0, 0.0); Gl.glVertex3d(i, j, 0);
                    Gl.glTexCoord2d(0.0, 1.0); Gl.glVertex3d(i + w, j, 0);
                    Gl.glTexCoord2d(1.0, 1.0); Gl.glVertex3d(i + w, j + w, 0);
                    Gl.glTexCoord2d(1.0, 0.0); Gl.glVertex3d(i, j + w, 0);
				}
			Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[1]);

            Gl.glBegin(Gl.GL_QUADS);
			w = 0.5;
            for (double j = 0; j < 25; j += w)
                for (double i = 0; i < 5; i += w)
				{
					Gl.glNormal3f( 1.0f, 0.0f, 0.0f);
                    Gl.glTexCoord2d(0.0, 0.0); Gl.glVertex3d(0, j, i);
                    Gl.glTexCoord2d(0.0, 1.0); Gl.glVertex3d(0, j, i + w);
                    Gl.glTexCoord2d(1.0, 1.0); Gl.glVertex3d(0, j + w, i + w);
                    Gl.glTexCoord2d(1.0, 0.0); Gl.glVertex3d(0, j + w, i);
				}
			Gl.glEnd();

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[1]);

            Gl.glBegin(Gl.GL_QUADS);
			w = 0.5;
            for (double j = 0; j < 25; j += w)
                for (double i = 0; i < 5; i += w)
				{
					Gl.glNormal3f( 0.0f, 1.0f, 0.0f);
                    Gl.glTexCoord2d(0.0, 0.0); Gl.glVertex3d(j, 0, i);
                    Gl.glTexCoord2d(0.0, 1.0); Gl.glVertex3d(j, 0, i + w);
                    Gl.glTexCoord2d(1.0, 1.0); Gl.glVertex3d(j + w, 0, i + w);
                    Gl.glTexCoord2d(1.0, 0.0); Gl.glVertex3d(j + w, 0, i);
				}
				Gl.glEnd();

                Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture[2]); 
			Gl.glPushMatrix();
			{
				w = 2;
				Gl.glTranslated(2,0.01, 1);
                Gl.glBegin(Gl.GL_QUADS);
					Gl.glNormal3d( 0.0f, 1.0f, 0.0f);
                    Gl.glTexCoord2d(0.0, 0.0); Gl.glVertex3d(0, 0, 0);
                    Gl.glTexCoord2d(0.0, 1.0); Gl.glVertex3d(0, 0, w);
                    Gl.glTexCoord2d(1.0, 1.0); Gl.glVertex3d(w, 0, w);
                    Gl.glTexCoord2d(1.0, 0.0); Gl.glVertex3d(w, 0, 0);
				Gl.glEnd();
			}
			Gl.glPopMatrix();

			Gl.glPushMatrix();
			{
				w = 2;
				Gl.glTranslated(0.01,4, 1);
                Gl.glBegin(Gl.GL_QUADS);
				Gl.glNormal3f( 1.0f, 0.0f, 0.0f);
                Gl.glTexCoord2d(0.0, 0.0); Gl.glVertex3d(0, 0, 0);
                Gl.glTexCoord2d(0.0, 1.0); Gl.glVertex3d(0, 0, w);
                Gl.glTexCoord2d(1.0, 1.0); Gl.glVertex3d(0, w, w);
                Gl.glTexCoord2d(1.0, 0.0); Gl.glVertex3d(0, w, 0);
                Gl.glEnd();
			}
			Gl.glPopMatrix();

            Gl.glDisable(Gl.GL_TEXTURE_2D);

			Gl.glEnd();
		}

        public static int DrawGLScene()									// Here's Where We Do All The Drawing
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);	// Clear The Screen And The Depth Buffer
            Gl.glLoadIdentity();

            //setPointCameraView(posX, posY, posZ);

            //computeCameraParam();

            Glu.gluLookAt(camPos[0], camPos[1], camPos[2], camCenter[0], camCenter[1], camCenter[2], 0, 0, 1);


            drawWalls(150, 150, 5, 0.3);

            float[] front_color1 = { (float)80.0 / 255, (float)100.0 / 255, (float)157.0 / 255, 1 };
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, front_color1);

            Gl.glPushMatrix();
            {
                Gl.glTranslated(quadrocopter.centerCoord[0], quadrocopter.centerCoord[1], quadrocopter.centerCoord[2]);
                Gl.glRotated(quadrocopter.angles[0], 1.0f, 0.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[1], 0.0f, 1.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[2], 0.0f, 0.0f, 1.0f);

                Create3DRect(0.11, 0.11, 0.01);
           
                Create3DRect(0.55, 0.03, 0.03);
            
                Create3DRect(0.03, 0.55, 0.03);
            }
            Gl.glPopMatrix();

            float[] front_color2 = { (float)50.0 / 255, (float)30.0 / 255, (float)250.0 / 255, 1 };
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, front_color2);

            Gl.glPushMatrix();
            {
                Gl.glTranslated(quadrocopter.centerCoord[0], quadrocopter.centerCoord[1], quadrocopter.centerCoord[2]);
                Gl.glRotated(quadrocopter.angles[0], 1.0f, 0.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[1], 0.0f, 1.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[2], 0.0f, 0.0f, 1.0f);

                Gl.glTranslated(0, 0.275 + 0.015, 0.02);
                Create3DRect(0.03, 0.03, 0.06);
            }
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            {
                Gl.glTranslated(quadrocopter.centerCoord[0], quadrocopter.centerCoord[1], quadrocopter.centerCoord[2]);
                Gl.glRotated(quadrocopter.angles[0], 1.0f, 0.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[1], 0.0f, 1.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[2], 0.0f, 0.0f, 1.0f);

                Gl.glTranslated(0.275 + 0.015, 0, 0.02);
                Create3DRect(0.03, 0.03, 0.06);
            }
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            {
                Gl.glTranslated(quadrocopter.centerCoord[0], quadrocopter.centerCoord[1], quadrocopter.centerCoord[2]);
                Gl.glRotated(quadrocopter.angles[0], 1.0f, 0.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[1], 0.0f, 1.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[2], 0.0f, 0.0f, 1.0f);

                Gl.glTranslated(-(0.275 + 0.015), 0, 0.02);
                Create3DRect(0.03, 0.03, 0.06);
            }
            Gl.glPopMatrix();

            float[] front_color4 = { (float)255.0 / 255, (float)0.0 / 255, (float)10.0 / 255, 1 };
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, front_color4);

            Gl.glPushMatrix();
            {
                Gl.glTranslated(quadrocopter.centerCoord[0], quadrocopter.centerCoord[1], quadrocopter.centerCoord[2]);
                Gl.glRotated(quadrocopter.angles[0], 1.0f, 0.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[1], 0.0f, 1.0f, 0.0f);
                Gl.glRotated(quadrocopter.angles[2], 0.0f, 0.0f, 1.0f);

                Gl.glTranslated(0, -(0.275 + 0.015), 0.02);
                Create3DRect(0.03, 0.03, 0.06);
            }
            Gl.glPopMatrix();

            /*
            Gl.glLoadIdentity();
            Gl.glColor3f(1.0f, 0, 0);
            Glut.glutWireSphere(2, 32, 32);
            */

            Gl.glFlush();

            //auxSolidBox(10,20,30);

            //Create3DRect(0,0,10,10,10,10);

            /*
            glColor3f(0.0, 0.0, 0.0);
            glBegin(GL_LINES);
            glVertex2d(-20,-20);
            glVertex2d(-20,300);
            glVertex2d(-20,-20);
            glVertex2d(300,-20);
            for(int i=-20; i<300;i+=20)
            {
            glColor3f(0.0, 0.0, 0.0);
            glVertex2d(i,-15);
            glVertex2d(i,-25);
            glColor3f(0.0, 0.0, 0.0);
            glVertex2d(-25,i);
            glVertex2d(-15,i);
            }
            glEnd();*/

            return 0;
        }
    }
}
