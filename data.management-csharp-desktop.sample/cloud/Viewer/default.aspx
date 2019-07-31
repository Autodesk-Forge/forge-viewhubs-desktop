<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="FPD.Sample.Cloud.Viewer._default" %>

<head>
    <meta name="viewport" content="width=device-width, minimum-scale=1.0, initial-scale=1, user-scalable=no" />
    <meta charset="utf-8">

    <!-- The Viewer CSS -->
    <link rel="stylesheet" href="https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/style.min.css" type="text/css">

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
    <title>DM test</title>
</head>
<body>
    <!-- The Viewer JS -->
    <script src="https://developer.api.autodesk.com/modelderivative/v2/viewers/7.*/viewer3D.js"></script>

    <!-- The Viewer will be instantiated here 
        This has to be placed before our scripts below, so that it's available
        by the time we call GuiViewer3D() -->
    <div id="MyViewerDiv"></div>

    <!-- Developer JS -->
    <script>
        var viewer, viewerApp;
        function showModel(urn, proxyRoute) {
            var options = {
                env: 'Local', //'AutodeskProduction',
                //useCredentials: false,
                endpoint: proxyRoute,
            };
            var documentId = 'urn:' + urn;

            try {
                Autodesk.Viewing.Initializer(options, function onInitialized() {
                    viewer = new Autodesk.Viewing.GuiViewer3D(document.getElementById('MyViewerDiv'));
                    viewer.start();
                    Autodesk.Viewing.Document.load(documentId, onDocumentLoadSuccess, onDocumentLoadFailure);
                });
            } catch (ex) {
                console.log('Initializer failed');
                console.log(ex);
            }
        }

        function onDocumentLoadSuccess(doc) {
            console.log('onDocumentLoadSuccess');
            var viewables = doc.getRoot().getDefaultGeometry();
            viewer.loadDocumentNode(doc, viewables).then(i => {
                // documented loaded, any action?
                
                console.log('loadDocumentNode -> then');
                console.log(i);
            }).catch(i => {
                console.log('loadDocumentNode -> catch');
                console.log(i);
            });
        }

        function onDocumentLoadFailure(viewerErrorCode) {
            console.log('onDocumentLoadFailure');
        }
    </script>
    <form runat="server"></form>
</body>
