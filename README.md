# FeedR
**Sample** (and rather **simple**) .NET6 **microservices** solution which acts as the data aggregator for the different feeds.
The overall repository structure consists of the following projects located under `src` directory:

- Gateway - API gateway providing a public facade for the underlying, internal services
- Aggregator - core service responsible for aggregating the data from different feeds
- Notifier - supporting service responsible for sending the notifications
- Feeds
  - News - sample feed providing the worldwide news
  - Quotes - sample feed providing quotes (e.g. currency)
  - Weather - sample feed providing the weather related data
