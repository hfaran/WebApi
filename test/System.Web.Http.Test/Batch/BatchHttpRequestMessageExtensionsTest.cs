﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Net.Http;
using System.Web.Http.Controllers;
using Microsoft.TestCommon;

namespace System.Web.Http.Batch
{
    public class BatchHttpRequestMessageExtensionsTest
    {
        [Theory]
        [InlineData("MS_HttpRouteData")]
        [InlineData("MS_DisposableRequestResources")]
        [InlineData("MS_SynchronizationContext")]
        [InlineData("MS_HttpConfiguration")]
        [InlineData("MS_HttpBatchContext")]
        public void CopyBatchRequestProperties_IgnoresSpecialProperties(string specialPropertyName)
        {
            // Arrange
            string notSpecialPropertyName = "SomeProperty";
            HttpRequestMessage baseRequest = new HttpRequestMessage();
            HttpRequestMessage childRequest = new HttpRequestMessage();

            baseRequest.Properties.Add(notSpecialPropertyName, 42);
            baseRequest.Properties.Add(specialPropertyName, 42);

            // Act
            childRequest.CopyBatchRequestProperties(baseRequest);

            // Assert
            Assert.Contains(notSpecialPropertyName, childRequest.Properties.Keys);
            Assert.DoesNotContain(specialPropertyName, childRequest.Properties.Keys);
        }

        [Fact]
        public void CopyBatchRequestProperties_AddsBatchHttpRequestContext()
        {
            using (HttpRequestMessage subRequest = new HttpRequestMessage())
            using (HttpRequestMessage batchRequest = new HttpRequestMessage())
            {
                HttpRequestContext expectedOriginalContext = new HttpRequestContext();
                subRequest.SetRequestContext(expectedOriginalContext);

                // Act
                BatchHttpRequestMessageExtensions.CopyBatchRequestProperties(subRequest, batchRequest);

                // Assert
                HttpRequestContext context = subRequest.GetRequestContext();
                Assert.IsType<BatchHttpRequestContext>(context);
                BatchHttpRequestContext typedContext = (BatchHttpRequestContext)context;
                Assert.Same(expectedOriginalContext, typedContext.BatchContext);
            }
        }
    }
}