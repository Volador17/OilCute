function [y,t1]=plsp1v(x,w,p,b,my)
% PLS1Ô¤²â³ÌÐò

z=x';
y=0;

[m,n]=size(p);
for i=1:n
    t1(:,i)=z*w(:,i);
    y=y+b(i)*t1(:,i);
    z=z-t1(:,i)*p(:,i)';
end
    
y=(y+my)'; 
  

