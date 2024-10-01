document.addEventListener("DOMContentLoaded", function () {
    // Attach the event listener when the modal is shown
    $('#basketModal').on('shown.bs.modal', function () {
        document.querySelectorAll(".remove").forEach(function (button) {
            button.addEventListener("click", function () {
                console.log("Attempting to remove from basket...");

                const token = getJwtTokenFromCookie();
                if (!token) {
                    alert("Please log in to remove games from the basket.");
                    return;
                }
                var decodedToken = jwt_decode(token);

                const gameId = button.getAttribute("data-game-id");
                const userId = decodedToken.nameid || decodedToken.id;

                console.log(`Removing game with ID: ${gameId}`);

                // Send the AJAX request to remove the game from the basket
                fetch(`https://localhost:7047/api/Basket/remove?userId=${userId}&gameId=${gameId}`, {
                    method: 'DELETE',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                })
                    .then(response => {
                        if (response.ok) {
                            // Remove the item with animation
                            const basketItem = button.closest('.basket-item');
                            if (basketItem) {
                                // Add the fade-out animation class
                                basketItem.classList.add('fade-out');
                                // Wait for the animation to complete, then remove the item from the DOM
                                basketItem.addEventListener('transitionend', function () {
                                    basketItem.remove();
                                    updateTotalPrice();
                                });
                            }
                        } else {
                            return response.text().then(errorText => {
                                throw new Error(errorText);
                            });
                        }
                    })
                    .catch(error => {
                      
                    });
            });
        });
    });

    // Function to get JWT token from the cookie
    function getJwtTokenFromCookie() {
        const name = "token=";
        const decodedCookie = decodeURIComponent(document.cookie);
        const cookies = decodedCookie.split(';');
        for (let i = 0; i < cookies.length; i++) {
            let cookie = cookies[i];
            while (cookie.charAt(0) === ' ') {
                cookie = cookie.substring(1);
            }
            if (cookie.indexOf(name) === 0) {
                return cookie.substring(name.length, cookie.length);
            }
        }
        return "";
    }

    // Function to update the total price after removing an item
    function updateTotalPrice() {
        let totalPrice = 0;
        document.querySelectorAll('.basket-item').forEach(function (item) {
            const priceText = item.querySelector('.basket-item-details p:nth-of-type(3)').textContent;
            const totalText = priceText.match(/Total: \$([0-9.]+)/);
            if (totalText && totalText[1]) {
                totalPrice += parseFloat(totalText[1]);
            }
        });

        // Update the total price in the DOM
        document.querySelector('.total-price p').textContent = `$${totalPrice.toFixed(2)}`;
    }
});
