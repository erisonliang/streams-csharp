# streams-csharp
A map reduce stream library for C#

Examples:
```
var numberOfOfficersWithTenYearsService = Stream.of(allDepartments)
  .flatMap(department => department.getStaffMembers())
  .filter(staff => staff.officer == true)
  .filter(staff => staff.yearsOfService >= 10)
  .count();
```

```
var sumOfEvenNumbersLessThanOneHundred = Stream.ofRange(1, 100)
  .filter(i => i % 2 == 0)
  .reduce((a, b) => a + b);
```

```
var lowerCaseAlpha = Stream.ofRange(0, 26)
  .map(i => 'a' + i)
  .toArray();
```

```
var pediatricPatientsByLastName = Stream.of(allPatients)
  .filter(p => p.age < 19)
  .sort((a, b) => a.lastName.CompareTo(b.lastName))
  .toArray();
```
