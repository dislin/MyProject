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
    }
}
