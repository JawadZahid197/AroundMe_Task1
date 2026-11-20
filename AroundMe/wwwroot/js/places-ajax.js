// D3 AJAX: $.get updates only the cards instead of refreshing the whole page.
$(function () {
    const $form = $("#search-form");
    const $results = $("#place-results");
    const $status = $("#search-status");

    function updatePlaces(search, categoryId) {
        $status.text("Loading places...");

        $.ajax({
            url: "/Home/SearchResults",
            type: "GET",
            data: { search: search, categoryId: categoryId },
            success: function (html) {
                $results.html(html);
                $status.text("Results updated without reloading the page.");
            },
            error: function () {
                $status.text("Could not load places. Please try again.");
            }
        });
    }

    $form.on("submit", function (event) {
        event.preventDefault();
        const search = $form.find("input[name='Search']").val();
        const categoryId = $form.find("input[name='CategoryId']").val() || 0;
        updatePlaces(search, categoryId);
    });

    $(".ajax-category").on("click", function (event) {
        event.preventDefault();
        $(".ajax-category").removeClass("active");
        $(this).addClass("active");

        const link = new URL(this.href);
        const categoryId = link.searchParams.get("categoryId") || 0;
        const search = $form.find("input[name='Search']").val();

        let $categoryInput = $form.find("input[name='CategoryId']");
        if (!$categoryInput.length) {
            $categoryInput = $("<input>", { type: "hidden", name: "CategoryId" });
            $form.append($categoryInput);
        }
        $categoryInput.val(categoryId);
        updatePlaces(search, categoryId);
    });
});
