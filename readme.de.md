# fritzptr

**fritzptr** ist ein lokaler DNS-over-HTTP(S)‑Dienst (DoH), der Reverse-DNS‑Anfragen (PTR) für IP-Adressen im Heimnetz intelligent beantwortet.

Statt klassischem rDNS greift fritzptr auf die FRITZ!Box (TR‑064) zu, ermittelt bekannte Netzwerkgeräte und liefert deren Namen als PTR‑Antwort zurück.

> Kurz: **IP rein → Gerätename raus**, direkt aus deinem Heimnetz.

---

## Motivation

Viele FRITZ!Box‑Modelle beantworten keine oder nur unvollständige rDNS‑Anfragen (`in-addr.arpa`, `ip6.arpa`).

Das führt zu:

* fehlenden Hostnamen in Logs
* unlesbaren Firewall‑/Proxy‑Einträgen
* schlechter Debuggability im Home‑Lab

**fritzptr** schließt diese Lücke. Es kann als upstream DNS Server für z.B. AdGuardHome verwendet werden.

---

## Features

* ✅ DNS over HTTPS (RFC 8484)
* ✅ PTR‑Auflösung für IPv4 (`in-addr.arpa`)
* ✅ FRITZ!Box‑Integration via TR‑064
* ✅ Cache für bekannte Hosts (IP → Name)

## Was kein feature wird

* HTTPS / TLS (Hol dir einen reverse proxy dafür)
* DNS über TCP oder UPD (Das hier sollte eh nicht dein primärer DNS Server sein, sondern eine Middleware, die deinen DNS Server bei der Namensauflösung im Heimnetz unterstützt, wenn du eine FRITZ!Box verwendest.)
* Fallback auf externen Resolver

---

## Namensauflösung (PTR)

### IPv4

```
4.1.168.192.in-addr.arpa → nas.fritz.box
```

### IPv6 (nicht unterstützt)

Da die FRITZ!Box keine Informationen zu IPv6 Adressen über TR-064 rausgibt, kann auch keine Namensauflösung über IPv6 stattfinden. Sollte Fritz diese Funktion jemals implementieren will ich diese nachreichen.

---

## Status

🚧 **Early Development / Proof of Concept**

* API & PTR‑Resolver: funktionsfähig
* FRITZ!Box‑Client: funktionsfähig
* Codebase: 99% vibe coded und unaufgeräumt
* Docker: geplant
* Logging: nicht existent, geplant

---

## Lizenz

MIT License