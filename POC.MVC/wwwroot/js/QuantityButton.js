<script>
    document.addEventListener('DOMContentLoaded', (event) => {
        function changeQuantity(amount) {
            const inputBox = document.querySelector('.quantity .input-box');
            const orderPriceElement = document.getElementById('order-price');
            const totalPriceContainer = document.getElementById('total-price-container');
            const totalPriceElement = document.getElementById('total-price');
            const unitPrice = parseFloat(orderPriceElement.value);

            let currentValue = parseInt(inputBox.value); // Get the current value from the input box
            const minValue = parseInt(inputBox.min);
            const maxValue = parseInt(inputBox.max);

            let newValue = currentValue + amount; // Update the value based on the amount (either +1 or -1)
            console.log(`Current Value: ${currentValue}, Amount: ${amount}, New Value: ${newValue}`); // Debugging line

            if (newValue < minValue) {
                newValue = minValue; // Ensure newValue doesn't go below the minimum value
            }
            if (newValue > maxValue) {
                newValue = maxValue; // Ensure newValue doesn't exceed the maximum value
            }

            inputBox.value = newValue; // Update the input box with the new value

            if (newValue > 1) {
                let newTotalPrice = unitPrice * newValue;
                totalPriceElement.textContent = newTotalPrice.toLocaleString('en-IN', { style: 'currency', currency: 'INR' });
                orderPriceElement.value = newTotalPrice.toFixed(2);
                totalPriceContainer.style.display = 'block';
            } else {
                totalPriceContainer.style.display = 'none';
                orderPriceElement.value = unitPrice.toFixed(2);
            }
        }

        window.changeQuantity = changeQuantity; // Make the function accessible globally
    });
</script>