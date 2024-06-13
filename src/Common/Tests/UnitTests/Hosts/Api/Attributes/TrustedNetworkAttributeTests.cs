﻿using Meta.Common.Contracts.Options;
using Meta.Common.Hosts.Attributes.TrustedNetwork;

namespace Meta.Common.Tests.UnitTests.Hosts.Api.Attributes
{
    /// <summary>
    /// Тесты атрибута доверенных сетей.
    /// </summary>
    internal class TrustedNetworkAttributeTests
    {
        [Test(Description = "При создании атрибута схема должна быть установлена в соответствующее наименование")]
        public void TrustedNetworkAttribute_Valid_AuthenticationSchemesIsEqualSchemeName()
        {
            // Act
            var attribute = new TrustedNetworkAttribute();

            // Assert
            Assert.That(attribute.AuthenticationSchemes, Is.EqualTo(TrustedNetworkOptions.Scheme));
        }
    }
}
