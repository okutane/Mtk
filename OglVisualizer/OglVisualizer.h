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
		HWND hwnd;
		HDC hdc;
		HGLRC hrc;

        Mesh ^_mesh;
        NormalsType _normalsType; //normals to use
        Point _translate; //translate vector
        double _phi, _theta; //polar coords;
        double distance;
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
                _translate = *value;
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
            _enableLightning = false;
            _translate.X = _translate.Y = _translate.Z = 0;
			_theta = _phi = 0;
            _lightDirection.x = 0;
            _lightDirection.y = 0;
            _lightDirection.z = 1;
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

			gluUnProject(x, y, 1, modelMatrix, projMatrix, viewport,
				&result.origin.X, &result.origin.Y, &result.origin.Z);
            Point lookAt;
            gluUnProject(x, y, 0, modelMatrix, projMatrix, viewport, &lookAt.X, &lookAt.Y, &lookAt.Z);
            result.direction = Vector::Normalize(lookAt - result.origin);

            return result;
        }
    private:
        void Reshape(int w, int h)
        {
            glViewport(0,0,(GLsizei) w, (GLsizei) h);
            glMatrixMode(GL_PROJECTION);
            glLoadIdentity();
            if (w<=h) 
                glOrtho(-1.5,1.5,-1.5*(GLfloat)h/(GLfloat)w,1.5*(GLfloat)h/(GLfloat)w,-10.0,10.0);
            else
                glOrtho(-1.5*(GLfloat)w/(GLfloat)h,1.5*(GLfloat)w/(GLfloat)h,-1.5,1.5,-10.0,10.0);
            glMatrixMode(GL_MODELVIEW);
            glLoadIdentity();
        }

        void polarView(GLdouble distance, GLdouble twist, GLdouble elevation, GLdouble azimuth)
        {
            double centerx, centery, centerz;
            double eyex, eyey, eyez;

            eyex = distance * Cos(-twist) * Cos(elevation);
            eyey = distance * Sin(-twist) * Cos(elevation);
            eyez = distance * Sin(elevation);
            centerx = centery = centerz = 0;

            gluLookAt(eyex, eyey, eyez, centerx, centery, centerz, 0,0,1);
        } 
	protected:
		virtual void OnPaint(System::Windows::Forms::PaintEventArgs ^e) override
		{
            glEnable(GL_CULL_FACE);
            if(_enableLightning)
            {
                glEnable(GL_LIGHTING);
                glEnable(GL_LIGHT0);
                glEnable(GL_DEPTH_TEST);
                glClearColor(0, 0, 0, 1);
                glClearColor(BackColor.R / 255.0, BackColor.G / 255.0, BackColor.B / 255.0, 1);
                glPolygonMode(GL_FRONT, GL_FILL);

                GLfloat lightPosition[] = {_lightDirection.x, _lightDirection.y, _lightDirection.z, 0};
                glLightfv(GL_LIGHT0, GL_POSITION, lightPosition);
            }
            else
            {
                glDisable(GL_LIGHTING);
                glDisable(GL_LIGHT0);
                glPolygonMode(GL_FRONT, GL_LINE);
                glClearColor(BackColor.R / 255.0, BackColor.G / 255.0, BackColor.B / 255.0, 1);
            }
			
			glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            glLoadIdentity();

            //Camera setup
            polarView(5, _phi, _theta, 0);

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
                        glVertex3d(vertex->Point);
                    }
                }
                glEnd();

                //Draw bounds
                glColor3f(0.1, 0.1, 0.1);
                glBegin(GL_LINES);
                for each(Matveev::Mtk::Core::Edge ^edge in _mesh->Edges)
                {
                    if(!edge->Pair)
                    {
                        glVertex3d(edge->Begin->Point);
                        glVertex3d(edge->End->Point);
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
                        glVertex3d(vertex->Point);
                        if(_drawPoints == OglVisualizer::DrawPoints::DrawWithNormals)
                        {
                            Point p = vertex->Point + 0.2 * (vertex->Normal);
                            glVertex3d(p);
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
                        Point p;
                        for each(Matveev::Mtk::Core::Vertex ^vert in face->Vertices)
                        {
                            p.X += (vert->Point.X) / 3;
                            p.Y += (vert->Point.Y) / 3;
                            p.Z += (vert->Point.Z) / 3;
                        }
                        glVertex3d(p);
                        p = p + 0.2 * (face->Normal);
                        glVertex3d(p);
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
                        glVertex3d(v->Point);
                    glEnd();
                }

                if(_edgeSelected)
                {
                    Matveev::Mtk::Core::Vertex ^b, ^e;
                    b = _edgeSelected->Begin;
                    e = _edgeSelected->End;
                    glColor3f(1, 0, 0);
                    glLineWidth(2);
                    glBegin(GL_LINES);
                    glVertex3d(b->Point);
                    glVertex3d(e->Point);
                    glEnd();
                    glLineWidth(1);
                }

                if(_faceSelected)
                {
                    glColor3f(1, 0, 0);
                    glPolygonMode(GL_FRONT, GL_FILL);
                    glBegin(GL_POLYGON);
                    for each(Matveev::Mtk::Core::Vertex ^vertex in _faceSelected->Vertices)
                        glVertex3d(vertex->Point);
                    glEnd();
                }
			}

            if(!SwapBuffers(hdc))
				throw gcnew Exception("Couldn't swap buffers!");            
		}

		void glVertex3d(Point point)
		{
			::glVertex3d(point.X, point.Y, point.Z);
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
			hwnd = (HWND)this->Handle.ToInt32();
			hdc = GetDC(hwnd);
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