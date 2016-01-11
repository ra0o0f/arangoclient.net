## 0.7.2

### Feautres

- Add support for aql upsert command
- Support for Adding custom `JsonConverter`
- Enum serialization to `string` or underlying `integer`
- Add Json.net `MetadataPropertyHandling` into `DatabaseSetting`
- Add DropCollection method
- Add http timeout setting
- Add bulk import

### Bug fixes

- Fix resolving members name in LINQ queries when use `CamelCase` naming convention
- Fix change tracker `CreateChangedDocument` when old object is null or not defined
- Fix setting proxy for mono support (thanks to @jjchiw)
- Fix resolving client version for mono support (thanks to @jjchiw)
- Fix deserializing objects that use Json.net `TypeNameHandling`