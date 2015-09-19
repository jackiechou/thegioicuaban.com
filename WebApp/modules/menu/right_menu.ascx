<%@ Control Language="C#" AutoEventWireup="true" Inherits="user_controls_admin_controls_right_menu" Codebehind="right_menu.ascx.cs" %>
   
 <style type="text/css">
    body{
      font-family:Arial, Helvetica, sans-serif;
      margin:10px;
    }
    .wrapper{
      position:relative;
      font-family:Arial, Helvetica, sans-serif;
      padding-top:10px;
      padding-right:50px;
      width:80%;      
      margin:auto
    }
     
     /* .wrapper h1{
      font-family:Arial, Helvetica, sans-serif;
      font-size:26px;
    }
    .longText{
      margin-top:20px;
      width:600px;
      font:18px/24px Arial, Helvetica, sans-serif;
      color:gray;
    }*/
     
    span.btn{
      padding:10px;
      display:inline-block;
      cursor:pointer;
      font:12px/14px Arial, Helvetica, sans-serif;
      color:#aaa;
      background-color:#eee;
      -moz-border-radius:10px;
      -webkit-border-radius:10px;
      -moz-box-shadow:#999 2px 0px 3px;
      -webkit-box-shadow:#999 2px 0px 3px;
    }
    span.btn:hover{
      background-color:#000;
    }

      /*
      custom style for extruder
      */

      #extruderRight .flap
      {
          -moz-border-radius:10px 0 0 10px;
            -moz-box-shadow:-2px 0 5px #999999;
            background:none repeat scroll 0 0 #000000;
            color:white;
            font:12px/16px  Arial,Helvetica,sans-serif; 
            left:-18px !important;
            padding:10px 10px 10px 0px !important;
            position:absolute;
            top:0;
            width:10px;
      }
      
      
       #extruderBottom .flap
      {
          -moz-border-radius:10px 10px 0 0;
            -moz-box-shadow:2px 0 2px #999999;
            background:none repeat scroll 0 0 #000000;
            color:white;
            cursor:pointer;
            display:block;
            font:12px/16px  Arial,Helvetica,sans-serif;            
            margin:1px auto auto;
            padding:0 5px 5px;
            position:relative;
            text-align:center;
            text-shadow:2px 2px 2px #333333;
            height:10px;
            width:50px;     
        
      }
     
      
    .extruder.left.a .flap{
      font-size:18px;
      color:white;
      top:0;
      padding:10px 0 10px 10px;
      background:#772B14;
      width:30px;
      position:absolute;
      right:0;
      -moz-border-radius:0 10px 10px 0;
      -webkit-border-top-right-radius:10px;
      -webkit-border-bottom-right-radius:10px;
      -moz-box-shadow:#666 2px 0px 3px;
      -webkit-box-shadow:#666 2px 0px 3px;
    }

    .extruder.left.a .content{
      border-right:3px solid #772B14;
    }
    .extruder.left.a .flap .flapLabel{
      background:#772B14;
    }
    
    #extruderBottom{     
      left:83% !important;    
    }    
 
   /*.extruder.top .optionsPanel .panelVoice a:hover{
      color:#fff;
      background: url("elements/black_op_30.png");
      border-bottom:1px solid #000;
    }
    .extruder.top .optionsPanel .panelVoice a{
      border-bottom:1px solid #000;
    }
    }*/
  </style>

  <link href="Scripts/jquery.mb.extruder.2.0/css/mbExtruder.css" media="all" rel="stylesheet" type="text/css"/>
  <script type="text/javascript" src="Scripts/jquery.mb.extruder.2.0/inc/jquery1.4.2.js"></script>
  <script type="text/javascript" src="Scripts/jquery.mb.extruder.2.0/inc/jquery.hoverIntent.min.js"></script>
  <script type="text/javascript" src="Scripts/jquery.mb.extruder.2.0/inc/jquery.metadata.js"></script>
  <script type="text/javascript" src="Scripts/jquery.mb.extruder.2.0/inc/jquery.mb.flipText.js"></script>
  <script type="text/javascript" src="Scripts/jquery.mb.extruder.2.0/inc/mbExtruder.js"></script>
  <script type="text/javascript">
      jQuery(document).ready(function ($) {

//          $("#extruderTop").buildMbExtruder({
//              positionFixed: false,
//              position: "top",
          //    width: 300,
//              extruderOpacity: 1,
//              autoCloseTime: 0,
//              hidePanelsOnClose: false,
//              onExtOpen: function () { },
//              onExtContentLoad: function () { },
//              onExtClose: function () { }
//          });

          $("#extruderBottom").buildMbExtruder({
              position: "bottom",
              //width: 300,
              width: 250,
              extruderOpacity: 1,
              onExtOpen: function () { },
              onExtContentLoad: function () { },
              onExtClose: function () { }
          });


//          $("#extruderLeft").buildMbExtruder({
//              position: "left",
//              width: 300,
//              extruderOpacity: .8,
//              hidePanelsOnClose: false,
//              accordionPanels: false,
//              onExtOpen: function () { },
//              onExtContentLoad: function () { $("#extruderLeft").openPanel(); },
//              onExtClose: function () { }
//          });

          $("#extruderRight").buildMbExtruder({
              position: "right",
              //width: 300,
              width: 250,
              extruderOpacity: .8,
              textOrientation: "tb",
              onExtOpen: function () { },
              onExtContentLoad: function () { },
              onExtClose: function () { }
          });
      });

  </script>   

<div>
      <%-- <div class="wrapper">
            <div>         
                 <span class="btn" onclick="$('#extruderLeft').openMbExtruder(true);$('#extruderLeft').openPanels()">Open</span>
                 <span class="btn" onclick="$('#extruderLeft').closeMbExtruder();">Close</span>    
            </div>
       </div>
       <div id="extruderLeft" class="{title:'Menu', url:'Menu_Left.aspx'}"></div>
       
       <div id="extruderTop" class="{title:'extruder top', url:'parts/extruderTop.html'}"></div>    
       --%>
       <div id="extruderRight" class="{title:'Menu', url:'Menu_Left.aspx'}"></div> 
       <div id="extruderBottom" class="{title:'Menu ', url:'Menu_RightBottom.aspx'}"></div>        
</div>