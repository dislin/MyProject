using Authentication.Role.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Authentication.Role.Entity;

namespace AuthenticationTest
{
    
    
    /// <summary>
    ///This is a test class for RoleServiceTest and is intended
    ///to contain all RoleServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RoleServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetRoleByID
        ///</summary>
        [TestMethod()]
        public void GetRoleByIDTest()
        {
            //Arrange
            //db config path D:\\MyProject\\MvcTest\\MvcTest\\Configuration\\
            RoleService_Accessor target = new RoleService_Accessor(); // TODO: Initialize to an appropriate value
            int id = 1; // TODO: Initialize to an appropriate value

            int expected = 1; // TODO: Initialize to an appropriate value
            RoleEntity actual;

            //Act
            actual = target.GetRoleByID(id);

            //Assert
            Assert.AreEqual(expected, actual.idnum);
        }

        /// <summary>
        ///A test for CreateRole
        ///</summary>
        [TestMethod()]
        public void CreateRoleTest()
        {
            //db config path D:\\MyProject\\MvcTest\\MvcTest\\Configuration\\
            RoleService_Accessor target = new RoleService_Accessor(); // TODO: Initialize to an appropriate value
            RoleEntity role = new RoleEntity()
            {
                name = "TestLeo",
                status = Authentication.Role.Enum.RoleEnum.RoleStatusEnum.InActive,
                creater = "testor",
                permission = "none"
            };
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CreateRole(role);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for UpdateRole
        ///</summary>
        [TestMethod()]
        public void UpdateRoleTest()
        {
            //db config path D:\\MyProject\\MvcTest\\MvcTest\\Configuration\\
            RoleService_Accessor target = new RoleService_Accessor(); // TODO: Initialize to an appropriate value
            RoleEntity role = target.GetRoleByID(3); // TODO: Initialize to an appropriate value
            role.name = "Yang Hang";
            bool expected = true; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.UpdateRole(role);
            Assert.AreEqual(expected, actual);
        }
    }
}
