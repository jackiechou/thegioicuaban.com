<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_slideshow_bk.ascx.cs" Inherits="WebApp.portals.news.controls.uc_slideshow_bk" %>

<script type="text/javascript">
     jQuery(document).ready(function () {
         //Load Slide
         var slideWidth = 980;
         var slides = $('.slide');
         var numberOfSlides = slides.length;
         var currentPosition = parseInt($('#currentPositionSlide').val());
         $('#slidesContainer').css('overflow', 'hidden');
         slides
          .wrapAll('&lt;div id="slideInner"&gt;&lt;/div&gt;')
     	  .css({
     	      'float': 'left',
     	      'width': slideWidth
     	  });
         $('#slideInner').css('width', slideWidth * numberOfSlides);
//         $('#slideshow')
//          .prepend('&lt;span class="control" id="leftControl"&gt;Trang trước&lt;/span&gt;')
//          .append('&lt;span class="control" id="rightControl"&gt;Trang sau&lt;/span&gt;');
         $('.control')
         .bind('click', function () {
             currentPosition = ($(this).attr('id') == 'rightControl') ? currentPosition + 1 : currentPosition - 1;
             $('#currentPositionSlide').val(currentPosition);
             if (currentPosition == numberOfSlides) {
                 currentPosition = 0;
                 $('#currentPositionSlide').val(0);
             }
             if (currentPosition == -1) {
                 currentPosition = numberOfSlides - 1;
                 $('#currentPositionSlide').val(parseInt(numberOfSlides) - 1);
             }
             $('#slideInner').animate({
                 'marginLeft': slideWidth * (-currentPosition)
             });
         });
         setInterval(function () { loadSlider() }, 9000);
     });

     //Function Slide
     function loadSlider() {
         var slides = $('.slide');
         var slideWidth = 980;
         var numberOfSlides = slides.length;
         var currentPositionSlide = $('#currentPositionSlide').val();
         currentPositionSlide = parseInt(currentPositionSlide) + 1;
         $('#currentPositionSlide').val(currentPositionSlide);
         //Return position if go to last slide
         if (currentPositionSlide == numberOfSlides) {
             currentPositionSlide = 0;
             $('#currentPositionSlide').val(0);
         }
         // Move slideInner using margin-left
         $('#slideInner').animate({
             'marginLeft': slideWidth * (-currentPositionSlide)
         });
     }
 </script>

<div id="slideshow">           
    <span class="control" id="leftControl">Trang trước</span>
    <div id="slidesContainer" style="overflow: hidden;">
        <div id="slideInner">           
            <asp:Literal ID="ltrSlide" runat="server"></asp:Literal>
        </div>
        <input type="hidden" value="0" id="currentPositionSlide" ></div>
    <span class="control" id="rightControl">Trang sau</span>
</div>
<div class="clear"></div>