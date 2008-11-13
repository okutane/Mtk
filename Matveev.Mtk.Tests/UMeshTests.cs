using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Utilities;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class UMeshTests
    {
        [Test]
        public void TestCreateFan()
        {
            MockRepository mocks = new MockRepository();
            Mesh mesh = mocks.StrictMock<Mesh>();
            Vertex[] v = new Vertex[5];
            for (int i = 0; i < 5; i++)
            {
                v[i] = mocks.StrictMock<Vertex>();
            }
            Expect.Call(mesh.CreateFace(v[0], v[1], v[2])).Return(mocks.StrictMock<Face>());
            Expect.Call(mesh.CreateFace(v[0], v[2], v[3])).Return(mocks.StrictMock<Face>());
            Expect.Call(mesh.CreateFace(v[0], v[3], v[4])).Return(mocks.StrictMock<Face>());

            mocks.ReplayAll();
            mesh.CreateFan(v);
            mocks.VerifyAll();
        }

        [Test]
        public void TestCreateClosedFan()
        {
            MockRepository mocks = new MockRepository();
            Mesh mesh = mocks.StrictMock<Mesh>();
            Vertex[] v = new Vertex[5];
            for (int i = 0; i < 5; i++)
            {
                v[i] = mocks.StrictMock<Vertex>();
            }
            Expect.Call(mesh.CreateFace(v[0], v[1], v[2])).Return(mocks.StrictMock<Face>());
            Expect.Call(mesh.CreateFace(v[0], v[2], v[3])).Return(mocks.StrictMock<Face>());
            Expect.Call(mesh.CreateFace(v[0], v[3], v[4])).Return(mocks.StrictMock<Face>());
            Expect.Call(mesh.CreateFace(v[0], v[4], v[1])).Return(mocks.StrictMock<Face>());

            mocks.ReplayAll();
            mesh.CreateClosedFan(v);
            mocks.VerifyAll();
        }
    }
}
