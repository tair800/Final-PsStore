function openPage(pageName, elmnt, color) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablink");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].style.backgroundColor = "";
    }
    document.getElementById(pageName).style.display = "block";
    elmnt.style.backgroundColor = color;
}

// Get the element with id="defaultOpen" and click on it
document.getElementById("defaultOpen").click();
// Get the promo image element
const promoImage = document.getElementById("promoImage");

// Get all images in the squares section
const squareImages = document.querySelectorAll(".square-img");

// Add click event to all square images
squareImages.forEach(img => {
    img.addEventListener("click", function () {
        // Change the promo image's src to the clicked image's src
        promoImage.src = this.src;
    });
});

