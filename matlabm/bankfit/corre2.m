function [r]=corre2(x,y);
[m,n]=size(x);
if m<n
    x=x';
end
[m,n]=size(y);
if m<n
    y=y';
end
[m,n]=size(x);
[p,q]=size(y);
if m==p
    m=p;
else
error('Matrix dimensions must agree.m=p')
end
r=sum(x.*y)^2/(sum(x.*x)*sum(y.*y));
r=r^2;
