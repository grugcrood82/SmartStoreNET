#Release Notes

##PayMark 3.2.2.0
###New Features
* New instalments provider.

##PayMark 3.1.5.3
###Improvements
* PayMark PLUS: now up to 10 more third party payment methods are allowed by PayMark.

##PayMark 3.1.5.2
###Improvements
* PayMark PLUS: additionally store access data in the database.

##PayMark 3.1.5.1
###Bugfixes
* PayMark Express: Checkout attributes were always ignored.

##PayMark 3.0.0.3
###Bugfixes
* PayMark PLUS: Fixed #1200 Invalid request if the order amount is zero. "Amount cannot be zero" still occurred.

##PayMark 3.0.0.2
###New Features
* PayMark Standard: New settings "UsePayMarkAddress" and "IsShippingAddressRequired" to avoid payment rejection due to address validation.
###Bugfixes
* PayMark PLUS: Fixed HTTP 401 "Unauthorized" when calling PatchShipping.

##PayMark 2.6.0.7
###Bugfixes
* PayMark Express: Fixed net price issue.

##PayMark 2.6.0.6
###Bugfixes
* PayMark PLUS: Skip payment if cart total is zero.
* PayMark PLUS: Do not display payment wall if method is filtered
###Improvements
* PayMark PLUS: Log more information in case of a request failure.

##PayMark 2.6.0.5
###Bugfixes
* PayMark PLUS: Fixed "Cannot perform runtime binding on a null reference" when rendering the payment wall.

##PayMark 2.6.0.4
###Bugfixes
* PayMark PLUS: Excluding tax issue. Fixed "Transaction amount details (subtotal, tax, shipping) must add up to specified amount total".

##PayMark 2.6.0.3
###Bugfixes
* PayMark PLUS: Integration review through PayMark
* PayMark PLUS: Generic attribute caching problem. Fixed "Item amount must add up to specified amount subtotal (or total if amount details not specified)".

##PayMark 2.6.0.1
###Improvements
* Added PayMark partner attribution Id as request header

##PayMark 2.5.0.2
###New Features
* PayMark PLUS payment provider

##PayMark 2.5.0.1
###Bugfixes
* PayMark Standard: The order amount transmitted to PayMark was wrong if gift cards or reward points were applied

##PayMark 2.2.0.4
###New Features
* Option for API security protocol
* Option to display express checkout button in mini shopping cart
* Support for partial refunds
* Option whether IPD may change the payment status of an order
###Bugfixes
* "The request was aborted: Could not create SSL/TLS secure channel." See https://devblog.PayMark.com/upcoming-security-changes-notice/
* PayMark Express: Void and refund out of function ("The transaction id is not valid")

##PayMark 2.2.0.3
###New Features
* Option to add order note when order total validation fails

##PayMark 2.2.0.2
###Improvements
* Redirecting to payment provider performed by core instead of plugin

##PayMark 2.2.0.1
###New Features
* Supports order list label for new incoming IPNs

##PayMark 1.22
###Bugfixes
* PayMark Standard provider now using shipping rather than billing address if shipping is required

##PayMark 1.21
###Improvements
* Multistore configuration