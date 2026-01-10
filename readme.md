# fritzptr

**fritzptr** is a local DNS-over-HTTP(S) (DoH) service that intelligently responds to reverse DNS (PTR) queries for IP addresses in your home network.

Instead of classic rDNS, fritzptr accesses the FRITZ!Box (TR-064), identifies known network devices, and returns their names as a PTR response.

> In short: **IP in → device name out**, directly from your home network.

---

## Motivation

Many FRITZ!Box models do not respond to rDNS queries (`in-addr.arpa`, `ip6.arpa`) or only respond incompletely.

This leads to:

* missing host names in logs
* unreadable firewall/proxy entries
* poor debugability in the home lab

**fritzptr** closes this gap. It can be used as an upstream DNS server for AdGuardHome, for example.

---

## Features

* ✅ DNS over HTTPS (RFC 8484)
* ✅ PTR resolution for IPv4 (`in-addr.arpa`)
* ✅ FRITZ!Box integration via TR-064
* ✅ Cache for known hosts (IP → name)

## What will not be a feature

* HTTPS / TLS (Get a reverse proxy for this)
* DNS over TCP or UPD (This should not be your primary DNS server anyway, but rather middleware that supports your DNS server with name resolution in your home network if you use a FRITZ!Box.)
* Fallback to external resolver

---

## Name resolution (PTR)

### IPv4

```
4.1.168.192.in-addr.arpa → nas.fritz.box
```

### IPv6 (not supported)

Since the FRITZ!Box does not provide any information about IPv6 addresses via TR-064, name resolution via IPv6 is not possible. If Fritz ever implements this feature, I will add it.

---

## Status

🚧 **Early Development / Proof of Concept**

* API & PTR resolver: functional
* FRITZ!Box client: functional
* Codebase: 99% vibe coded and untidy
* Docker: planned
* Logging: non-existent, planned

---

## License

MIT License