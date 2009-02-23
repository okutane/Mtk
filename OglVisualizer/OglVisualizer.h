// OglVisualizer.h

#pragma once

#pragma comment(lib, "opengl32.lib")
#pragma comment(lib, "glu32.lib")

#include <windows.h>
#include <gl/gl.h>
#include <gl/glu.h>
#include "enums.h"

using namespace System;
using namespace System::Windows::Forms;
using namespace System::Collections::Generic;
using namespace Matveev::Mtk::Core;

const double pi = System::Math::PI;

inline double Sin(double x)
{
    return System::Math::Sin(x);
}

inline double Cos(double x)
{
    return System::Math::Cos(x);
}

namespace OglVisualizer
{
    public ref class Visualizer : public System::Windows::Forms::UserControl
	{	
	private:
		HDC hdc;
		HGLRC hrc;

        Mesh ^_mesh;
        NormalsType _normalsType; //normals to use
        Point ^_translate; //translate vector
        double _phi, _theta; //polar coords;
        Vector _lightDirection; //light direction

		DrawPoints _drawPoints;
        bool _drawFaceNormals;
        bool _enableLightning;

        IEnumerable<Vertex^> ^_vertsMarked;
        Edge ^_edgeSelected;
        Face ^_faceSelected;

	public:        
        property Mesh ^Mesh
        {
			void set(Matveev::Mtk::Core::Mesh ^value)
            {
                _mesh = value;
                Invalidate();
            }
        }

        property Point ^Translate
		{
			Point ^get()
			{
				return _translate;
			}
			void set(Point ^value)
			{
                _translate = value;
				Invalidate();
			}
		}

        property double Phi
        {
            double get()
            {
                return _phi;
            }
            void set(double value)
            {
                _phi = value;
                Invalidate();
            }
        }

        property double Theta
        {
            double get()
            {
                return _theta;
            }
            void set(double value)
            {
                _theta = value;
                if(_theta > pi / 2)
                    _theta = pi / 2;
                else if(_theta < -pi / 2)
                    _theta = -pi / 2;
                Invalidate();
            }
        }

        property NormalsType NormalsToUse
        {
            NormalsType get()
            {
                return _normalsType;
            }
            void set(NormalsType normalsType)
            {
                _normalsType = normalsType;
                Invalidate();
            }
        }
		
		property DrawPoints DrawPoints
		{
            OglVisualizer::DrawPoints get()
			{
				return _drawPoints;
			}
			void set(OglVisualizer::DrawPoints value)
			{
				_drawPoints = value;
				Invalidate();
			}
		}

        property bool DrawFaceNormals
        {
            bool get()
            {
                return _drawFaceNormals;
            }
            void set(bool value)
            {
                _drawFaceNormals = value;
                Invalidate();
            }
        }

        property bool EnableLightning
        {
            bool get()
            {
                return _enableLightning;
            }
            void set(bool value)
            {
                _enableLightning = value;
                Invalidate();
            }
        }
		
        property IEnumerable<Vertex^> ^MarkedVerts
		{
            void set(IEnumerable<Vertex^> ^value)
			{
				_vertsMarked = value;
                _edgeSelected = nullptr;
                _faceSelected = nullptr;
				Invalidate();
			}
		}
        property Matveev::Mtk::Core::Edge ^SelectedEdge
		{
            void set(Edge ^value)
			{
				_edgeSelected = value;
                _vertsMarked = nullptr;
                _faceSelected = nullptr;
				Invalidate();
			}
		}
        property Matveev::Mtk::Core::Face ^SelectedFace
		{
            void set(Face ^value)
			{
				_faceSelected = value;
                _edgeSelected = nullptr;
                _vertsMarked = nullptr;
				Invalidate();
			}
		}

	public:
		Visualizer()
		{
			_translate = gcnew Point(0, 0, 0);
			_theta = _phi = 0;
            _lightDirection.x = 0;
            _lightDirection.y = 0;
            _lightDirection.z = -1;
		}

        Ray RayThroughScreen(int x, int y)
        {
            x = x;
            y = Height-y;
            Ray result;

            GLdouble modelMatrix[16], projMatrix[16];
            GLint viewport[4];

            glGetDoublev(GL_MODELVIEW_MATRIX, modelMatrix);
            glGetDoublev(GL_PROJECTION_MATRIX, projMatrix);
            glGetIntegerv(GL_VIEWPORT, viewport);

			double ox;
			double oy;
			double oz;
			gluUnProject(x, y, 1, modelMatrix, projMatrix, viewport,
				&ox, &oy, &oz);
			result.origin = Point(ox, oy, oz);
			gluUnProject(x, y, 0, modelMatrix, projMatrix, viewport, &ox, &oy, &oz);
            Point lookAt = Point(ox, oy, oz);
            result.direction = Vector::Normalize(lookAt - result.origin);

            return result;
        }
    private:
		static void Reshape(int w, int h)
		{
			float diameter = System::Math::Sqrt(3);
			glViewport(0, 0, (GLsizei)w, (GLsizei)h);
			glMatrixMode(GL_PROJECTION);
			glLoadIdentity();
			GLdouble zNear = 1;
			GLdouble zFar = zNear + diameter * 2;
			GLdouble left = -diameter;
			GLdouble right = diameter;
			GLdouble top = -diameter;
			GLdouble bottom = diameter;
			double aspect = (double)w / (double)h;
			if (aspect < 1)
			{
				bottom /= aspect;
				top /= aspect;
			}
			else
			{
				left *= aspect;
				right *= aspect;
			}
			glOrtho(left, right, bottom, top, zNear, zFar);
			glMatrixMode(GL_MODELVIEW);
			glLoadIdentity();
		}

		static void PolarView(GLdouble twist, GLdouble elevation)
		{
			float diameter = System::Math::Sqrt(3);
			double distance = diameter * 2;
			double centerx, centery, centerz;
			double eyex, eyey, eyez;

			eyex = distance * Cos(twist) * Cos(elevation);
			eyey = distance * Sin(twist) * Cos(elevation);
			eyez = distance * Sin(elevation);
			centerx = centery = centerz = 0;

			gluLookAt(eyex, eyey, eyez, centerx, centery, centerz, 0, 0, 1);
		} 
	protected:
		virtual void OnPaint(System::Windows::Forms::PaintEventArgs ^e) override
		{
            glEnable(GL_CULL_FACE);
            glEnable(GL_DEPTH_TEST);
			
            if(_enableLightning)
            {
                glEnable(GL_LIGHTING);
                glEnable(GL_LIGHT0);
                glClearColor(0, 0, 0, 1);
                glClearColor(BackColor.R / 255.0f, BackColor.G / 255.0f, BackColor.B / 255.0f, 1);
				glPolygonMode(GL_FRONT, GL_FILL);

                GLfloat lightPosition[] = {(float)_lightDirection.x, (float)_lightDirection.y,
					(float)_lightDirection.z, 0};
                glLightfv(GL_LIGHT0, GL_POSITION, lightPosition);
            }
            else
            {
                glDisable(GL_LIGHTING);
                glDisable(GL_LIGHT0);
				glPolygonMode(GL_FRONT, GL_LINE);
                glClearColor(BackColor.R / 255.0f, BackColor.G / 255.0f, BackColor.B / 255.0f, 1);
            }
			
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            glLoadIdentity();

            //Camera setup
			PolarView(_phi, _theta);

			if(_mesh)
			{                
				glColor3f(0.5, 0.5, 0.5);
                glBegin(GL_TRIANGLES);
                for each(Matveev::Mtk::Core::Face ^face in _mesh->Faces)
                {
                    if(_enableLightning && _normalsType == NormalsType::FaceNormals)
                        glNormal3d(face->Normal.x, face->Normal.y, face->Normal.z);
                    for each(Matveev::Mtk::Core::Vertex ^vertex in face->Vertices)
                    {
                        if(_enableLightning && _normalsType == NormalsType::VertexNormals)
                            glNormal3d(vertex->Normal.x, vertex->Normal.y, vertex->Normal.z);
                        PutVertex(vertex->Point);
                    }
                }
                glEnd();

                //Draw bounds
                glColor3f(0.1f, 0.1f, 0.1f);
                glBegin(GL_LINES);
                for each(Matveev::Mtk::Core::Edge ^edge in _mesh->Edges)
                {
                    if(!edge->Pair)
                    {
                        PutVertex(edge->Begin->Point);
                        PutVertex(edge->End->Point);
                    }
                }
                glEnd();

                if(_drawPoints != OglVisualizer::DrawPoints::DoNotDraw)
                {
                    glColor3f(0.5, 0.5, 0.5);
                    glPointSize(5);
                    glLineWidth(2);

                    glColor3f(0.5, 0.5, 0.5);
                    if(_drawPoints == OglVisualizer::DrawPoints::DrawWithNormals)
                        glBegin(GL_LINES);
                    else
                        glBegin(GL_POINTS);

                    for each(Matveev::Mtk::Core::Vertex ^vertex in _mesh->Vertices)
                    {
                        PutVertex(vertex->Point);
                        if(_drawPoints == OglVisualizer::DrawPoints::DrawWithNormals)
                        {
                            Point ^p = vertex->Point + 0.2 * (vertex->Normal);
                            PutVertex(p);
                        }
                    }

                    glEnd();
                    glPointSize(1);
                    glLineWidth(1);
                }

                if(_drawFaceNormals)
                {
                    glColor3f(0.5, 0.5, 0.5);
                    glLineWidth(2);

                    glColor3f(0.5, 0.5, 0.5);
                    glBegin(GL_LINES);

                    for each(Matveev::Mtk::Core::Face ^face in _mesh->Faces)
                    {
                        double x, y, z;
						x = 0;
						y = 0;
						z = 0;
                        for each(Matveev::Mtk::Core::Vertex ^vert in face->Vertices)
                        {
                            x += vert->Point.X;
                            y += vert->Point.Y;
                            z += vert->Point.Z;
                        }
						Point p = Point(x / 3, y / 3, z / 3);
                        PutVertex(p);
                        p = p + 0.2 * (face->Normal);
                        PutVertex(p);
                    }

                    glEnd();
                    glLineWidth(1);
                }
                
                if(_vertsMarked)
                {
                    glColor3f(1, 0, 0);
                    glPointSize(5);
                    glBegin(GL_POINTS);
                    for each(Matveev::Mtk::Core::Vertex ^v in _vertsMarked)
                        PutVertex(v->Point);
                    glEnd();
                }

                if(_edgeSelected)
                {
                    Matveev::Mtk::Core::Vertex ^b = _edgeSelected->Begin;
					Matveev::Mtk::Core::Vertex ^e = _edgeSelected->End;
                    glColor3f(1, 0, 0);
                    glLineWidth(2);
                    glBegin(GL_LINES);
                    PutVertex(b->Point);
                    PutVertex(e->Point);
                    glEnd();
					glPointSize(5);
					glBegin(GL_POINTS);
					PutVertex(e->Point);
					glEnd();
                    glLineWidth(1);
                }

                if(_faceSelected)
                {
                    glColor3f(1, 0, 0);
                    glPolygonMode(GL_FRONT, GL_FILL);
                    glBegin(GL_POLYGON);
                    for each(Matveev::Mtk::Core::Vertex ^vertex in _faceSelected->Vertices)
                        PutVertex(vertex->Point);
                    glEnd();
                }
			}

            if(!SwapBuffers(hdc))
			{
				throw gcnew Exception("Couldn't swap buffers!");            
			}
		}

		static void PutVertex(Point ^point)
		{
			::glVertex3d(point->X, point->Y, point->Z);
		}

		virtual void OnPaintBackground(System::Windows::Forms::PaintEventArgs ^e) override
		{
		}

		virtual void OnResize(System::EventArgs ^e) override
		{
            Reshape(this->Width, this->Height);
            Invalidate();
		}

		virtual void OnCreateControl() override
		{
			hdc = GetDC((HWND)this->Handle.ToInt32());
			if(hdc == 0)
				throw gcnew Exception("Couldn't get Device Context!");

			PIXELFORMATDESCRIPTOR pfd =
            {
                sizeof(PIXELFORMATDESCRIPTOR),
                1,
                PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER,
                PFD_TYPE_RGBA,
                24,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                32,
                0,
                0,
                PFD_MAIN_PLANE,
                0,
                0,
                0,
                0
            }; 

			int iPixelFormat = ChoosePixelFormat(hdc, &pfd);
			if(!iPixelFormat)
				throw gcnew Exception("Couldn't choose Pixel Format!");
 
			if(!SetPixelFormat(hdc, iPixelFormat, &pfd))
				throw gcnew Exception("Couldn't set Pixel Format!");

            hrc = wglCreateContext(hdc);
			if(hrc == 0)
				throw gcnew Exception("Couldn't create Rendering Context!");

            wglMakeCurrent(hdc, hrc);
            Reshape(this->Width, this->Height);
			Invalidate();
		}
	};
}
