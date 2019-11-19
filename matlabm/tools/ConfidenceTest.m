clear;
t = 0.6;
step = 0.001;
TQ = 0.998;
SQ = 0.99;

mySQ = t;
while t<1
    t= t+step;
    mySQ = [mySQ t];
end
t = 0.6;
myTQ = t;
while t<1
    t= t+step;
    myTQ = [myTQ t];
end

cv = zeros(length(myTQ),length(mySQ));
for r=1:length(myTQ)
   for c=1:length(mySQ)
       cv(r,c) = Confidence(myTQ(r),TQ,mySQ(c),SQ);
   end
end

