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

document.getElementById("defaultOpen").click();
const promoImage = document.getElementById("promoImage");

const squareImages = document.querySelectorAll(".square-img");

squareImages.forEach(img => {
    img.addEventListener("click", function () {

        promoImage.src = this.src;
    });
});

document.getElementById('searchForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const searchInput = document.getElementById('searchInput').value;
    const searchResults = document.getElementById('searchResults');

    if (searchInput.trim() === "") {
        searchResults.innerHTML = "<p>Please enter a search term.</p>";
        return;
    }

    fetch(`/Game/Search?title=${encodeURIComponent(searchInput)}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'text/html'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Search request failed');
            }
            return response.text();
        })
        .then(data => {
            searchResults.innerHTML = data;
        })
        .catch(error => {
            searchResults.innerHTML = "<p>Error occurred while searching.</p>";
            console.error('Error:', error);
        });
});

const searchInput = document.getElementById('searchInput');
const searchResults = document.getElementById('searchResults');
const searchModal = document.getElementById('searchModal');

searchModal.addEventListener('hidden.bs.modal', function () {
    searchInput.value = "";
    searchResults.innerHTML = "";
});






