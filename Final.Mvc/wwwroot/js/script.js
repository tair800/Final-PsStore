// Simple debounce function to limit API calls while typing
function debounce(func, delay) {
    let timer;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => func.apply(this, args), delay);
    };
}

// Debounced search function
const debouncedSearch = debounce(function () {
    const query = document.getElementById('searchInput').value;

    if (query.length < 1) {
        document.getElementById('searchResults').innerHTML = '<div class="text-center text-muted">Type for searching...</div>';
        return;
    }

    // Perform AJAX request to fetch the search results
    fetch(`/Game/Search?title=${query}`)
        .then(response => response.text()) // Expecting partial view HTML as response
        .then(data => {
            document.getElementById('searchResults').innerHTML = data; // Inject the partial view result
        })
        .catch(() => {
            document.getElementById('searchResults').innerHTML = '<div class="text-center text-danger">Error while searching. Try again later.</div>';
        });
}, 300);


