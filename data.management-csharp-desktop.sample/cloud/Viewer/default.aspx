<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="FPD.Sample.Cloud.Viewer._default" %>

<head>
    <meta name="viewport" content="width=device-width, minimum-scale=1.0, initial-scale=1, user-scalable=no" />
    <meta charset="utf-8">

    <!-- The Viewer CSS -->
    <link rel="stylesheet" href="https://developer.api.autodesk.com/derivativeservice/v2/viewers/style.min.css?v=v6.0" type="text/css">

    <!-- Developer CSS -->
    <style>
        body {
            margin: 0;
        }

        #MyViewerDiv {
            width: 100%;
            height: 100%;
            margin: 0;
            background-color: #F0F0F0;
        }
    </style>
</head>
<body>
    <!-- The Viewer JS -->
    <script src="https://developer.api.autodesk.com/derivativeservice/v2/viewers/viewer3D.min.js?v=v6.0"></script>

    <!-- Developer JS -->
    <script>
        var viewerApp;
        function showModel(urn, proxyRoute) {
            var options = {
                env: 'AutodeskProduction',
            };
            var documentId = 'urn:' + urn;
            Autodesk.Viewing.Initializer(options, function onInitialized() {
                Autodesk.Viewing.endpoint.setEndpointAndApi(proxyRoute, '', true);
                viewerApp = new Autodesk.Viewing.ViewingApplication('MyViewerDiv');
                viewerApp.registerViewer(viewerApp.k3D, Autodesk.Viewing.Private.GuiViewer3D);
                viewerApp.loadDocument(documentId, onDocumentLoadSuccess, onDocumentLoadFailure);
            });
        }

        function onDocumentLoadSuccess(doc) {

            // We could still make use of Document.getSubItemsWithProperties()
            // However, when using a ViewingApplication, we have access to the **bubble** attribute,
            // which references the root node of a graph that wraps each object from the Manifest JSON.
            var viewables = viewerApp.bubble.search({ 'type': 'geometry' });
            if (viewables.length === 0) {
                console.error('Document contains no viewables.');
                return;
            }

            // Choose any of the avialble viewables
            viewerApp.selectItem(viewables[0].data, onItemLoadSuccess, onItemLoadFail);
        }

        function onDocumentLoadFailure(viewerErrorCode) { }

        function onItemLoadSuccess(viewer, item) { }

        function onItemLoadFail(errorCode) { }

    </script>
    <form runat="server"></form>

    <!-- The Viewer will be instantiated here -->
    <div id="MyViewerDiv"></div>
</body>
